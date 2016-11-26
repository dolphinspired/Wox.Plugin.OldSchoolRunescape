using System.Collections.Generic;
using NUnit.Framework;

namespace Wox.Plugin.OldSchoolRunescape.Tests
{
    [TestFixture]
    public class PluginTests
    {
        private Main _main;

        [TestCase("runite ore")]
        public void TestPlugin(string search)
        {
            var query = new Query
            {
                ActionKeyword = "osrs",
                Terms = search.Split(' ')
            };

            if (_main == null)
            {
                _main = new Main();
            }

            List<Result> results = _main.Query(query);

            Assert.That(results, Is.Not.Null);
        }
    }
}
