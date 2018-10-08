using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Wox.Plugin.RuneScapeWiki.Tests
{
    [TestFixture]
    public class PluginTests
    {
        private static Main _program;

        [OneTimeSetUp]
        public void SetUp()
        {
            _program = new Main();
        }

        private static List<Result> RunQuery(string search)
        {
            var terms = search.Split(Query.TermSeperater.ToCharArray());

            var query = new Query
            {
                ActionKeyword = terms[0],
                Terms = terms
            };

            return _program.Query(query);
        }

        [TestCase("rsw runite ore")]
        [TestCase("osw runite ore")]
        public void TestApi(string search)
        {
            var results = RunQuery(search);

            Assert.That(results, Is.Not.Null);
            Assert.That(!results.First().Title.Contains("Error"));
        }

        [TestCase("rsw nex")]
        [TestCase("osw bronze longsword")]
        public void TestBrowserStart(string search)
        {
            var results = RunQuery(search);

            // Open the first result in the browser
            results[0].Action(new ActionContext());

            // If you got here, ^that didn't blow up. gz
            Assert.That(true);
        }

        [TestCase("rsw rune long", "Rune longsword")]
        [TestCase("osw zulrah's scales", "Zulrah's scales")]
        public void TestTitleMatch(string search, string expectedFirstTitle)
        {
            var results = RunQuery(search);

            Assert.That(results[0].Title, Is.EqualTo(expectedFirstTitle));
        }
    }
}
