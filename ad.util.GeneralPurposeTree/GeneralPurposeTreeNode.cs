using ad.util.GeneralPurposeTreeRepos;
using Ninject;
using System.Collections.Generic;

namespace ad.util {
    public class GeneralPurposeTreeNode<T> {
        #region fields
        private IList<GeneralPurposeTreeNode<T>> _childs;
        #endregion

        #region properties
        #region public properties
        public long RootId { get; set; }

        public long Id { get; set; }

        public long? ParentId { get; set; }

        public string Name { get; set; }

        public T Peer { get; set; }

        public GeneralPurposeTreeNode<T> Parent { get; set; }

        public IList<GeneralPurposeTreeNode<T>> Childs {
            get {
                if (_childs == null)
                    _childs = GetChildren();
                return _childs;
            }
            set { _childs = value; }
        }

        [Inject]
        public INodeRepository<T> Repository { get; set; }
        #endregion
        #endregion

        #region methods
        #region private methods
        private IList<GeneralPurposeTreeNode<T>> GetChildren() {
            if (Repository == null)
                return null;
            return TreeUtils.BuildSubtree(Repository.GetTreeUnderNode(this.Id), this);
        }
        #endregion
        #endregion
    }
}
