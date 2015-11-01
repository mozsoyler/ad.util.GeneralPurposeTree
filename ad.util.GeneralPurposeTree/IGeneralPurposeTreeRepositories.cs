using Insight.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ad.util;

namespace ad.util.GeneralPurposeTreeRepos {
    [Sql(Schema = "ad_util_GeneralPurposeTree")]
    public interface IRootRepository<T> : IDbConnection, IDbTransaction {
        //IList<GeneralPurposeTree<T>> FindRoots(
        //    GeneralPurposeTree<T> root = null,
        //    int? top = null,
        //    int? skip = null,
        //    string orderBy = null,
        //    string thenBy = null,
        //    string idOperator = "=",
        //    string nameOperator = "=",
        //    string initiatorOperator = "=");

        IList<GeneralPurposeTree<T>> FindRoots(
            long? id = null,
            string name = null,
            string initiator = null,
            int? top = null,
            int? skip = null,
            string orderBy = null,
            string thenBy = null,
            string idOperator = "=",
            string nameOperator = "=",
            string initiatorOperator = "=");

        IList<GeneralPurposeTree<T>> FindRoots(
            out long totalRows,
            long? id = null,
            string name = null,
            string initiator = null,
            int? top = null,
            int? skip = null,
            string orderBy = null,
            string thenBy = null,
            string idOperator = "=",
            string nameOperator = "=",
            string initiatorOperator = "=");

        void InsertRoot(GeneralPurposeTree<T> root);

        void InsertRoot(string name, string initiator);

        void InsertRoots(IEnumerable<GeneralPurposeTree<int>> roots);

        GeneralPurposeTree<T> SelectRoot(long id);

        void DeleteRoot(long id);

        void DeleteRoot(GeneralPurposeTree<T> root);

        void DeleteRoots(IEnumerable<long> ids);

        void DeleteRoots(IEnumerable<GeneralPurposeTree<T>> roots);

        void UpdateRoot(GeneralPurposeTree<T> root);

        IEnumerable<GeneralPurposeTreeNode<T>> GetTreeUnderRoot(long rootId, int? maxLevel = 15);
    }

    [Sql(Schema = "ad_util_GeneralPurposeTree")]
    public interface INodeRepository<T> : IDbConnection, IDbTransaction {
        IList<GeneralPurposeTreeNode<T>> FindNodes(
            long? rootId = null,
            long? id = null,
            long? parentId = null,
            string name = null,
            int? top = null,
            int? skip = null,
            string orderBy = null,
            string thenBy = null,
            string rootIdOperator = "=",
            string idOperator = "=",
            string parentIdOperator = "=",
            string nameOperator = "=");

        IList<GeneralPurposeTreeNode<T>> FindNodes(
            out long totalRows,
            long? rootId = null,
            long? id = null,
            long? parentId = null,
            string name = null,
            int? top = null,
            int? skip = null,
            string orderBy = null,
            string thenBy = null,
            string rootIdOperator = "=",
            string idOperator = "=",
            string parentIdOperator = "=",
            string nameOperator = "=");

        void InsertNode(GeneralPurposeTreeNode<T> node);

        void InsertNode(string name, string initiator);

        void InsertNodes(IEnumerable<GeneralPurposeTreeNode<int>> nodes);

        GeneralPurposeTreeNode<T> SelectNode(long id);

        void DeleteNode(long id);

        void DeleteNode(GeneralPurposeTreeNode<T> node);

        void DeleteNodes(IEnumerable<long> ids);

        void DeleteNodes(IEnumerable<GeneralPurposeTreeNode<T>> nodes);

        void UpdateNode(GeneralPurposeTreeNode<T> node);

        IEnumerable<GeneralPurposeTreeNode<T>> GetTreeUnderNode(long id);

        IEnumerable<GeneralPurposeTreeNode<T>> GetTreeUnderNode(GeneralPurposeTreeNode<T> node);
    }
}
