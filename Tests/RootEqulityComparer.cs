using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ad.util.test {
    internal class RootEqulityComparer<T> :EqualityComparer<GeneralPurposeTree<T>>{
        public override bool Equals(GeneralPurposeTree<T> x, GeneralPurposeTree<T> y) {
            return x.Id == y.Id && x.Name == y.Name && x.Initiator == y.Initiator;
        }

        public override int GetHashCode(GeneralPurposeTree<T> obj) {
            return obj.GetHashCode();
        }
    }
}
