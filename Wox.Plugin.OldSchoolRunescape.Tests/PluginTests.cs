using System.Collections.Generic;
using NUnit.Framework;

namespace Wox.Plugin.OldSchoolRunescape.Tests
{
    [TestFixture]
    public class PluginTests
    {
        private Main _main;

        private Main Program
        {
            get
            {
                if (_main == null)
                {
                    _main = new Main();
                }

                return _main;
            }
        }

        [TestCase("runite ore")]
        public void TestApi(string search)
        {
            var query = new Query
            {
                ActionKeyword = "osrs",
                Terms = search.Split(' ')
            };

            List<Result> results = Program.Query(query);

            Assert.That(results, Is.Not.Null);
        }

        [TestCase("bronze longsword")]
        public void TestBrowserStart(string search)
        {
            var query = new Query
            {
                ActionKeyword = "osrs",
                Terms = search.Split(' ')
            };

            List<Result> results = Program.Query(query);

            results[0].Action(new ActionContext());

            // If you got here, ^that didn't blow up. gz
            Assert.That(true);
        }
    }
}
