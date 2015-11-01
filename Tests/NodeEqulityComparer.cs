using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ad.util.test {
    internal class NodeEqulityComparer<T> : EqualityComparer<GeneralPurposeTreeNode<T>> {
        public override bool Equals(GeneralPurposeTreeNode<T> x, GeneralPurposeTreeNode<T> y) {
            return x.Id == y.Id && x.Name == y.Name && x.ParentId == y.ParentId &&
                x.Peer.Equals(y.Peer) && x.RootId == y.RootId;
        }

        public override int GetHashCode(GeneralPurposeTreeNode<T> obj) {
            return obj.GetHashCode();
        }
    }
}
