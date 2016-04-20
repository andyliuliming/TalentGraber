using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Macrodeek.Model;
using GithubGraberLib;
using GithubGraberLib.Domain;
using ExtractBase.RestApi;
using System;
using System.Data.Entity;
using Macrodeek.MacrodeekCommon;
using StarDustCommon.Util;
using Macrodeek.AzureEnvironment;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private GoldModelContainer db = new GoldModelContainer();

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole is running");
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            bool result = base.OnStart();
            Trace.TraceInformation("WorkerRole has been started");
            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole is stopping");
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            base.OnStop();
            Trace.TraceInformation("WorkerRole has stopped");
        }

        private GithubFeed Parse(string url)
        {
            GithubFeed githubFeed = new GithubFeed();
            string[] array = url.Split('/');
            githubFeed.owner = array[3];
            githubFeed.repo = array[4].Split('.')[0];
            return githubFeed;
        }

        private string getEmptyOrValue(string value)
        {
            return value == null ? string.Empty : value;
        }

        private void InterateChange<T>(IOrderedQueryable<T> iqueryable, Action<T> action)
        {
            int skipSize = 0;
            int batchSize = 10;
            while (true)
            {
                IQueryable<T> currentContributersToRepo = iqueryable.Skip(skipSize).Take(batchSize);
                if (currentContributersToRepo != null)
                {
                    List<T> contributerList = currentContributersToRepo.ToList();
                    if (contributerList.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        skipSize += 10;
                        foreach (T item in contributerList)
                        {
                            using (var transaction = db.Database.BeginTransaction())
                            {
                                action(item);
                                db.SaveChanges();
                                transaction.Commit();
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void ResetContributersToRepo(GithubRepo repo)
        {
            IOrderedQueryable<ContributerToRepo> orderedQueryable =
                (IOrderedQueryable<ContributerToRepo>)db.ContributerToRepoes.Where(ctp => ctp.RepoId == repo.Id).OrderBy(ctp => ctp.Id);
            this.InterateChange<ContributerToRepo>(orderedQueryable, new Action<ContributerToRepo>(ctp =>
            {
                ctp.CalculatingCommitNumber = 0;
            }));
        }

        private void SetValueForContributerToRepo(GithubRepo repo)
        {
            IOrderedQueryable<ContributerToRepo> orderedQueryable =
                (IOrderedQueryable<ContributerToRepo>)db.ContributerToRepoes.Where(ctp => ctp.RepoId == repo.Id).OrderBy(ctp => ctp.Id);
            this.InterateChange<ContributerToRepo>(orderedQueryable, new Action<ContributerToRepo>(ctp =>
            {
                ctp.CacheCommitNumber = ctp.CalculatingCommitNumber;
            }));
        }

        private void ResetTalentCandidateCommits()
        {
            this.InterateChange<TalentCandidate>(db.TalentCandidates.OrderBy(tc => tc.Id), new Action<TalentCandidate>(tc =>
            {
                tc.CalculatingTotalCommits = 0;
            }));
        }

        private void SetValueForTalentCandidateCommits()
        {
            this.InterateChange<TalentCandidate>(db.TalentCandidates.OrderBy(tc => tc.Id), new Action<TalentCandidate>(tc =>
            {
                tc.CacheTotalCommits = tc.CalculatingTotalCommits;
            }));
        }

        RestApiCaller<List<CommitDetail>> commitInfoCaller = new RestApiCaller<List<CommitDetail>>(ApiFormats.BaseUri);

        RestApiCaller<User> userInfoCaller = new RestApiCaller<User>(ApiFormats.BaseUri);

        AccountResource accountResource = new AccountResource();

        private void RealWork()
        {
            commitInfoCaller.AccountResource = accountResource;
            userInfoCaller.AccountResource = accountResource;

            this.InterateChange<TalentCandidate>(db.TalentCandidates.OrderBy(tc => tc.Id), new Action<TalentCandidate>(tc =>
            {
                string urlParameters = string.Format(ApiFormats.UserApi, tc.Login);
                Trace.TraceWarning(string.Format("getting the user {0}", tc.Login));
                User user = userInfoCaller.CallApi("get", urlParameters);
                if (user != null)
                {
                    tc.Company = getEmptyOrValue(user.company);
                    tc.Email = getEmptyOrValue(user.email);
                    tc.Followers = getEmptyOrValue(user.followers);
                    tc.FollowersUrl = getEmptyOrValue(user.followers_url);
                    tc.Location = getEmptyOrValue(user.location);
                    tc.Name = getEmptyOrValue(user.name);
                    tc.ReposUrl = getEmptyOrValue(user.repos_url);
                    tc.Timestamp = DateTime.UtcNow;
                }
                else
                {
                    Trace.TraceError("user result is null for: " + urlParameters);
                }
            }));

            this.ResetTalentCandidateCommits();
            foreach (GithubRepo githubRepo in db.GithubRepoes.ToList())
            {
                // clear the calculating number of the repository
                this.ResetContributersToRepo(githubRepo);
                int startPage = 0;
                int perPage = 100;

                GithubFeed githubFee = this.Parse(githubRepo.Url);
                string repoBaseUri = string.Format(ApiFormats.CommitRelativePathPattern, githubFee.owner, githubFee.repo);

                while (true)
                {
                    string urlParameters = repoBaseUri + "?page=" + startPage + "&per_page=" + perPage;
                    List<CommitDetail> pagedCommitDetails = commitInfoCaller.CallApi("get", urlParameters);
                    if (pagedCommitDetails == null)
                    {
                        break;
                    }
                    else if (pagedCommitDetails.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        for (int i = 0; i < pagedCommitDetails.Count; i++)
                        {
                            if (pagedCommitDetails[i].author != null)
                            {
                                string loginString = pagedCommitDetails[i].author.login;
                                TalentCandidate candidate = db.TalentCandidates.Where(
                                    (tc) => tc.Login == loginString).FirstOrDefault();
                                if (candidate == null)
                                {
                                    using (var transaction = db.Database.BeginTransaction())
                                    {
                                        candidate = new TalentCandidate();
                                        candidate.Company = string.Empty;
                                        candidate.Email = string.Empty;
                                        candidate.Followers = string.Empty;
                                        candidate.FollowersUrl = string.Empty;
                                        candidate.Location = string.Empty;
                                        candidate.Login = pagedCommitDetails[i].author.login;
                                        candidate.Name = string.Empty;
                                        candidate.ReposUrl = string.Empty;
                                        candidate.Timestamp = DateTime.UtcNow;
                                        db.TalentCandidates.Add(candidate);
                                        db.SaveChanges();
                                        transaction.Commit();
                                    }
                                }
                                ContributerToRepo contributerInfo = null;
                                using (var transaction = db.Database.BeginTransaction())
                                {
                                    contributerInfo = db.ContributerToRepoes.Where(ctp => ctp.RepoId == githubRepo.Id && ctp.TalentCandidateId == candidate.Id).FirstOrDefault();
                                    if (contributerInfo == null)
                                    {
                                        contributerInfo = new ContributerToRepo();
                                        contributerInfo.RepoId = githubRepo.Id;
                                        contributerInfo.TalentCandidateId = candidate.Id;
                                        contributerInfo.RepoUrl = githubRepo.Url;
                                        contributerInfo.TalentCandidateName = (candidate.Name == null ? string.Empty : candidate.Name);
                                        db.ContributerToRepoes.Add(contributerInfo);
                                        db.SaveChanges();
                                    }
                                    contributerInfo.RepoUrl = githubRepo.Url;
                                    contributerInfo.TalentCandidateName = (candidate.Name == null ? string.Empty : candidate.Name);
                                    contributerInfo.CalculatingCommitNumber += 1;
                                    candidate.CalculatingTotalCommits += 1;
                                    db.SaveChanges();
                                    transaction.Commit();
                                }
                            }
                        }
                        startPage++;
                    }

                }
                this.SetValueForContributerToRepo(githubRepo);
            }


            this.SetValueForTalentCandidateCommits();
        }

        private void KeepWarm()
        {
            try
            {
                string baseUrl = AppSettingsProvider.GetSetting(FieldNameUtil.GetMemberName((AzureSettingsNames c) => c.ProductServiceAddress));
                RestApiCaller<string> warmersCaller = new RestApiCaller<string>(baseUrl);
                warmersCaller.Get("/odata/Warmers", false);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            int currentStep = 0;
            int stepForKeepWarm = 1;
            int stepForRealWork = 60;
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Trace.TraceWarning("Working");
                    if ((currentStep % stepForKeepWarm) == 0)
                    {
                        this.KeepWarm();
                    }
                    if ((currentStep % stepForRealWork) == 0)
                    {
                        this.RealWork();
                    }
                    currentStep++;
                    // deplay for 1 min.
                    await Task.Delay(1000 * 60 * 1 * 1);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    await Task.Delay(1000 * 60 * 1 * 1);
                }
            }
        }
    }
}
