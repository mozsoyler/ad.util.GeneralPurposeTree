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

    public class NodeRepositoryTests : IDisposable {
        #region fields
        private GeneralPurposeTree<int> _root;
        private bool _disposed;
        #endregion

        #region constructors
        public NodeRepositoryTests() {
            DbUtils.ClearAll();
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                _root = DbUtils.CreateARoot(repo);
            }
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
        public void CanCreateTopNode() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                GeneralPurposeTreeNode<int> node;
                long idBeforeInserted;
                DbUtils.CreateANode(repo, out node, out idBeforeInserted, _root.Id);
                Assert.NotEqual(idBeforeInserted, node.Id);
            }
        }

        [Fact]
        public void CanSelectNode() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                GeneralPurposeTreeNode<int> insertedNode = DbUtils.CreateANode(repo, _root.Id);
                var selectedNode = repo.SelectNode(insertedNode.Id);
                Assert.NotNull(selectedNode);
                Assert.Equal(insertedNode.Id, selectedNode.Id);
            }
        }

        [Fact]
        public void CanFindNodes() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                List<long> ids = new List<long>();
                ids.Add(DbUtils.CreateANode(repo, _root.Id).Id);
                ids.Add(DbUtils.CreateANode(repo, _root.Id, name: "Deneme2").Id);
                ids.Add(DbUtils.CreateANode(repo, _root.Id, name: "Deneme3").Id);
                var nodes = repo.FindNodes();
                Assert.Equal(ids, nodes.Select(r => r.Id));
            }
        }

        [Fact]
        public void CanCreateMultipleNodes() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                List<GeneralPurposeTreeNode<int>> insertedNodes = new List<GeneralPurposeTreeNode<int>>();
                insertedNodes.Add(new GeneralPurposeTreeNode<int> { RootId = _root.Id, Name = "A" });
                insertedNodes.Add(new GeneralPurposeTreeNode<int> { RootId = _root.Id, Name = "B" });
                insertedNodes.Add(new GeneralPurposeTreeNode<int> { RootId = _root.Id, Name = "C" });
                repo.InsertNodes(insertedNodes);
                var foundNodes = repo.FindNodes();
                Assert.Equal(insertedNodes, foundNodes, new NodeEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanFindByName() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                List<GeneralPurposeTreeNode<int>> origNodes = new List<GeneralPurposeTreeNode<int>>();
                origNodes.Add(DbUtils.CreateANode(repo, _root.Id));
                origNodes.Add(DbUtils.CreateANode(repo, _root.Id, name: "X1"));
                origNodes.Add(DbUtils.CreateANode(repo, _root.Id, name: "X2"));
                var foundNodes = repo.FindNodes(name: "X%", nameOperator: "LIKE");
                Assert.Equal(origNodes.Where(r => r.Name.StartsWith("X")), foundNodes, new NodeEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanRemoveNodeById() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                GeneralPurposeTreeNode<int> insertedNode;
                long idBeforeInserted;
                DbUtils.CreateANode(repo, out insertedNode, out idBeforeInserted, _root.Id);
                var id = insertedNode.Id;
                repo.DeleteNode(id);
                var selectedNode = repo.SelectNode(id);
                Assert.Null(selectedNode);
            }
        }

        [Fact]
        public void CanRemoveNodeByInstance() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                GeneralPurposeTreeNode<int> insertedNode;
                long idBeforeInserted;
                DbUtils.CreateANode(repo, out insertedNode, out idBeforeInserted, _root.Id);
                var id = insertedNode.Id;
                repo.DeleteNode(insertedNode);
                var selectedNode = repo.SelectNode(id);
                Assert.Null(selectedNode);
            }
        }

        [Fact]
        public void CanRemoveMultipleNodesById() {
            List<long> ids = new List<long>();
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                ids.Add(DbUtils.CreateANode(repo, _root.Id).Id);
                ids.Add(DbUtils.CreateANode(repo, _root.Id, name: "Deneme2").Id);
                ids.Add(DbUtils.CreateANode(repo, _root.Id, name: "Deneme3").Id);
                repo.DeleteNodes(ids);
                var foundNodes = repo.FindNodes();
                Assert.Equal(new List<GeneralPurposeTreeNode<int>>(), foundNodes, new NodeEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanRemoveMultipleNodesByInstance() {
            List<GeneralPurposeTreeNode<int>> ids = new List<GeneralPurposeTreeNode<int>>();
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                ids.Add(DbUtils.CreateANode(repo, _root.Id));
                ids.Add(DbUtils.CreateANode(repo, _root.Id, name: "Deneme2"));
                ids.Add(DbUtils.CreateANode(repo, _root.Id, name: "Deneme3"));
                repo.DeleteNodes(ids);
                var foundNodes = repo.FindNodes();
                Assert.Equal(new List<GeneralPurposeTreeNode<int>>(), foundNodes, new NodeEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanUpdateNode() {
            const string UpdatedName = "Değiştirilen";
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                GeneralPurposeTreeNode<int> insertedNode;
                long idBeforeInserted;
                DbUtils.CreateANode(repo, out insertedNode, out idBeforeInserted, _root.Id);
                insertedNode.Name = UpdatedName;
                repo.UpdateNode(insertedNode);
                var selectedNode = repo.SelectNode(insertedNode.Id);
                Assert.NotNull(selectedNode);
                Assert.Equal(insertedNode, selectedNode, new NodeEqulityComparer<int>());
            }
        }

        [Fact]
        public void CanAddChildNodes() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                var topNode = DbUtils.CreateANode(repo, _root.Id);
                var firstLevel = DbUtils.CreateANode(repo, _root.Id, parentId: topNode.Id, name: "X1");
                var secondLevel = DbUtils.CreateANode(repo, _root.Id, parentId: firstLevel.Id, name: "X2");
                var selectedNodes = repo.FindNodes();
                Assert.Equal(new GeneralPurposeTreeNode<int>[] { topNode, firstLevel, secondLevel },
                    selectedNodes, new NodeEqulityComparer<int>());
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
