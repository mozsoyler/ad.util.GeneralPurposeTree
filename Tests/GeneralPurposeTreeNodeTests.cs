using System;
using Xunit;
using ad.util.GeneralPurposeTreeRepos;
using System.Collections.Generic;

namespace ad.util.test {
    public class GeneralPurposeTreeNodeTests : IDisposable {
        #region fields
        private bool _disposed;
        private GeneralPurposeTree<int> _root; 
        #endregion

        #region constructors
        public GeneralPurposeTreeNodeTests() {
            DbUtils.ClearAll();
            using (var repo = DbUtils.GetRepo<IRootRepository<int>>()) {
                _root = DbUtils.CreateARoot(repo); 
            }
        } 
        #endregion

        #region methods
        #region test methods
        [Fact]
        public void GetNodeSubTree() {
            using (var repo = DbUtils.GetRepo<INodeRepository<int>>()) {
                var top1 = DbUtils.CreateANode<int>(repo, _root.Id, name: "T1");
                var top2 = DbUtils.CreateANode<int>(repo, _root.Id, name: "T2");
                var node21 = DbUtils.CreateANode<int>(repo, _root.Id, name: "N21", parentId: top2.Id);
                var node11 = DbUtils.CreateANode<int>(repo, _root.Id, name: "N11", parentId: top1.Id);
                var node22 = DbUtils.CreateANode<int>(repo, _root.Id, name: "N22", parentId: top2.Id);
                var node12 = DbUtils.CreateANode<int>(repo, _root.Id, name: "N12", parentId: top1.Id);
                var node211 = DbUtils.CreateANode<int>(repo, _root.Id, name: "N211", parentId: node21.Id);

                var top = new List<GeneralPurposeTreeNode<int>> { top1, top2 };
                var level11 = new List<GeneralPurposeTreeNode<int>> { node11, node12 };
                var level12 = new List<GeneralPurposeTreeNode<int>> { node21, node22 };
                var level121 = new List<GeneralPurposeTreeNode<int>> { node211 };

                Assert.Equal(level12, top2.Childs, new NodeEqulityComparer<int>());
                Assert.Equal(level121, top2.Childs[0].Childs, new NodeEqulityComparer<int>());
                Assert.Equal(0, top2.Childs[1].Childs.Count);
            }
        }
        #endregion
        #region public methods
        public void Dispose() {
            Dispose(true);
        }
        #endregion

        #region private methods
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
