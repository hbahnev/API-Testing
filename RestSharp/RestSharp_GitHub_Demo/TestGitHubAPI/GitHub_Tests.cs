using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestGitHubAPI
{
    public class GitHub_Tests
    {
        private RestClient client;
        private RestRequest request;

        [SetUp]
        public void Setup()
        {
            client = new RestClient("https://api.github.com");
            client.Authenticator = new HttpBasicAuthenticator("hbahnev", "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");
            string url = "/repos/hbahnev/SoftUni-QA-Automation-2022/issues";

            request = new RestRequest(url);
        }

        private async Task<Issue> CreateIssue(string title, string body)
        {
            request.AddBody(new { title, body });
            var response = await client.ExecuteAsync(request, Method.Post);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);
            return issue;

        }
        private async Task<Issue> EditTitleIssue(string title)
        { 
            request.AddBody(new { title });
            var response = await client.ExecuteAsync(request, Method.Patch);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var issue = JsonSerializer.Deserialize<Issue>(response.Content);
            return issue;
        }

        [Test]
        public async Task APIRequestStatusCode()
        {
            var response = await client.ExecuteAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task APIRequestGetAllIssues()
        {
            var response = await client.ExecuteAsync(request);
            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);
            Assert.That(issues.Count > 1);
            foreach (var item in issues)
            {
                Assert.Greater(item.id, 0);
                Assert.Greater(item.number, 0);
                Assert.IsNotEmpty(item.title);
            }
        }

        [Test]
        public async Task APIRequestCreateIssue()
        {
            string title = "RestSharp Issue for editing";
            string body = "this is issue body";
            var issue = await CreateIssue(title, body);
            Assert.That(issue.id > 0);
            Assert.That(issue.number > 0);
            Assert.IsNotEmpty(issue.title);
        }
        [Test]
        public async Task APIRequestEditExistIssue()
        {
            string newTitle = "Edited from RestSharp";
            var editedIssue = await EditTitleIssue(newTitle);
            Assert.AreEqual(newTitle, editedIssue.title);
        }
        [Test]
        public async Task APIRequestRetriveAllCommentsFromIssue()
        {
            var response = await client.ExecuteAsync(request);
            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);
            Assert.Greater(issues.Count, 0);
            foreach (var issue in issues)
            {
                Assert.Greater(issue.id, 0);
                Assert.IsNotEmpty(issue.body);
            }
        }
    }
}