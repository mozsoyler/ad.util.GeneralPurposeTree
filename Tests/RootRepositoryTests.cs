using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ad.util;
using Insight.Database;
using ad.util.GeneralPurposeTreeRepos;
using System.Configuration;

namespace ad.util.test {

    public class RootRepositoryTests : IDisposable {
        #region fields
        private bool _disposed;
        #endregion

        #region constructors
        public RootRepositoryTests() {
            DbUtils.ClearAll();
        }
        #endregion

        #region methods
        #region public methods
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region test methods
        [Fact]
        public void CanCreateRoot() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                GeneralPurposeTree<int> root;
                long idBeforeInserted;
                DbUtils.CreateARoot(repo, out root, out idBeforeInserted);
                Assert.NotEqual(idBeforeInserted, root.Id);
            }
        }

        [Fact]
        public void CanSelectRoot() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                GeneralPurposeTree<int> insertedRoot = DbUtils.CreateARoot(repo);
                var selectedRoot = repo.SelectRoot(insertedRoot.Id);
                Assert.NotNull(selectedRoot);
                Assert.Equal(insertedRoot.Id, selectedRoot.Id);
            }
        }

        [Fact]
        public void CanFindRoots() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                List<long> ids = new List<long>();
                ids.Add(DbUtils.CreateARoot(repo).Id);
                ids.Add(DbUtils.CreateARoot(repo, name: "Deneme2").Id);
                ids.Add(DbUtils.CreateARoot(repo, name: "Deneme3").Id);
                var roots = repo.FindRoots();
                Assert.Equal(ids, roots.Select(r => r.Id));
            }
        }

        [Fact]
        public void CanCreateMultipleRoots() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                List<GeneralPurposeTree<int>> insertedRoots = new List<GeneralPurposeTree<int>>();
                insertedRoots.Add(new GeneralPurposeTree<int> { Name = "A", Initiator = "X" });
                insertedRoots.Add(new GeneralPurposeTree<int> { Name = "B", Initiator = "Y" });
                insertedRoots.Add(new GeneralPurposeTree<int> { Name = "C", Initiator = "Z" });
                repo.InsertRoots(insertedRoots);
                var foundRoots = repo.FindRoots();
                Assert.Equal(insertedRoots, foundRoots, new RootEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanFindByName() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                List<GeneralPurposeTree<int>> origRoots = new List<GeneralPurposeTree<int>>();
                origRoots.Add(DbUtils.CreateARoot(repo));
                origRoots.Add(DbUtils.CreateARoot(repo, name: "X1"));
                origRoots.Add(DbUtils.CreateARoot(repo, name: "X2"));
                var foundRoots = repo.FindRoots(name: "X%", nameOperator: "LIKE");
                Assert.Equal(origRoots.Where(r => r.Name.StartsWith("X")), foundRoots, new RootEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanRemoveRootById() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                GeneralPurposeTree<int> insertedRoot;
                long idBeforeInserted;
                DbUtils.CreateARoot(repo, out insertedRoot, out idBeforeInserted);
                var id = insertedRoot.Id;
                repo.DeleteRoot(id);
                var selectedRoot = repo.SelectRoot(id);
                Assert.Null(selectedRoot);
            }
        }

        [Fact]
        public void CanRemoveRootByInstance() {
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                GeneralPurposeTree<int> insertedRoot;
                long idBeforeInserted;
                DbUtils.CreateARoot(repo, out insertedRoot, out idBeforeInserted);
                var id = insertedRoot.Id;
                repo.DeleteRoot(insertedRoot);
                var selectedRoot = repo.SelectRoot(id);
                Assert.Null(selectedRoot);
            }
        }

        [Fact]
        public void CanRemoveMultipleRootsById() {
            List<long> ids = new List<long>();
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                ids.Add(DbUtils.CreateARoot(repo).Id);
                ids.Add(DbUtils.CreateARoot(repo, name: "Deneme2").Id);
                ids.Add(DbUtils.CreateARoot(repo, name: "Deneme3").Id);
                repo.DeleteRoots(ids);
                var foundRoots = repo.FindRoots();
                Assert.Equal(new List<GeneralPurposeTree<int>>(), foundRoots, new RootEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanRemoveMultipleRootsByInstance() {
            List<GeneralPurposeTree<int>> ids = new List<GeneralPurposeTree<int>>();
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                ids.Add(DbUtils.CreateARoot(repo));
                ids.Add(DbUtils.CreateARoot(repo, name: "Deneme2"));
                ids.Add(DbUtils.CreateARoot(repo, name: "Deneme3"));
                repo.DeleteRoots(ids);
                var foundRoots = repo.FindRoots();
                Assert.Equal(new List<GeneralPurposeTree<int>>(), foundRoots, new RootEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanUpdateRoot() {
            const string UpdatedName = "Değiştirilen";
            const string UpdatedUser = "yeni-kullanıcı";
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                GeneralPurposeTree<int> insertedRoot;
                long idBeforeInserted;
                DbUtils.CreateARoot(repo, out insertedRoot, out idBeforeInserted);
                insertedRoot.Name = UpdatedName;
                insertedRoot.Initiator = UpdatedUser;
                repo.UpdateRoot(insertedRoot);
                var selectedRoot = repo.SelectRoot(insertedRoot.Id);
                Assert.NotNull(selectedRoot);
                Assert.Equal(insertedRoot, selectedRoot, new RootEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanCollectTreeInCorrectOrder() {
            using (var rootRepo = DbUtils.GetRepo<IRootRepository<int>>())
            using (var nodeRepo = DbUtils.GetRepo<INodeRepository<int>>()) {
                var root = DbUtils.CreateARoot<int>(rootRepo, name: "Root");
                var top1 = DbUtils.CreateANode<int>(nodeRepo, root.Id, name: "T1");
                var top2 = DbUtils.CreateANode<int>(nodeRepo, root.Id, name: "T2");
                var lvl11 = DbUtils.CreateANode<int>(nodeRepo, root.Id, name: "L11", parentId: top1.Id);
                var lvl12 = DbUtils.CreateANode<int>(nodeRepo, root.Id, name: "L12", parentId: top1.Id);
                var lvl21 = DbUtils.CreateANode<int>(nodeRepo, root.Id, name: "L21", parentId: top2.Id);
                var lvl211 = DbUtils.CreateANode<int>(nodeRepo, root.Id, name: "L211", parentId: lvl21.Id);
                List<GeneralPurposeTreeNode<int>> origList =
                    new List<GeneralPurposeTreeNode<int>> { top1, lvl11, lvl12, top2, lvl21, lvl211 };
                var actualList = rootRepo.GetTreeUnderRoot(root.Id, maxLevel: 10);
                Assert.Equal(origList, actualList, new NodeEqulityComparer<int>());
            }
        }
        #endregion

        #region private helper methods
        private void Dispose(bool disposing) {
            if (_disposed)
                return;
            if (disposing)
                DbUtils.ClearAll();
        }
        #endregion
        #endregion
    }
}
