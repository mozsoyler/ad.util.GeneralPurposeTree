using ad.util.GeneralPurposeTreeRepos;
using Insight.Database;
using System.Configuration;
using System.Data;
using System.Linq;
using Ninject;

namespace ad.util.test {
    static class DbUtils {
        #region properties
        #region public properties
        public static ConnectionStringSettings Database { get; private set; }

        public static IKernel Kernel { get; set; }
        #endregion
        #endregion

        #region constructors
        static DbUtils() {
            Kernel = new StandardKernel();
            SqlInsightDbProvider.RegisterProvider();
            Database = ConfigurationManager.ConnectionStrings["ad.util.GeneralPurposeTree"];
        }

        private static void BindRepoTypeToDb<T>() where T : class {
            if (Kernel.GetBindings(typeof(T)).FirstOrDefault() != null)
                return;
            Kernel.Bind<T>().ToMethod(_ => Database.As<T>());
        }
        #endregion

        #region methods
        #region public methods
        #region public static methods
        public static IDbConnection Open() {
            return Database.Open();
        }

        public static T GetRepo<T>() where T : class {
            return Database.As<T>();
        }

        public static void ClearAll() {
            using (var conn = DbUtils.Database.Open()) {
                conn.ExecuteSql("delete from [ad_util_GeneralPurposeTree].[Node]");
                conn.ExecuteSql("delete from [ad_util_GeneralPurposeTree].[Root]");
            }
        }

        public static void CreateARoot<T>(IRootRepository<T> repo, out GeneralPurposeTree<T> root, out long createdId,
            string name = "Deneme") {
            //root = new GeneralPurposeTree<T> { Name = name, Initiator = "murat" };
            BindRepoTypeToDb<IRootRepository<T>>();
            root = Kernel.Get<GeneralPurposeTree<T>>();
            root.Name = name;
            root.Initiator = "murat";
            createdId = root.Id;
            repo.InsertRoot(root);
        }

        public static GeneralPurposeTree<T> CreateARoot<T>(IRootRepository<T> repo, string name = "Deneme") {
            GeneralPurposeTree<T> insertedRoot;
            long idBeforeInserted;
            CreateARoot<T>(repo, out insertedRoot, out idBeforeInserted, name);
            return insertedRoot;
        }

        public static void CreateANode<T>(INodeRepository<T> repo, out GeneralPurposeTreeNode<T> node, out long createdId,
            long rootId, string name = "Deneme", long? parentId = null) {
            BindRepoTypeToDb<INodeRepository<T>>();
            //node = new GeneralPurposeTreeNode<T> { Name = name, RootId = rootId, ParentId = parentId };
            node = Kernel.Get<GeneralPurposeTreeNode<T>>();
            node.RootId = rootId;
            node.Name = name;
            node.ParentId = parentId;
            createdId = node.Id;
            repo.InsertNode(node);
        }

        public static GeneralPurposeTreeNode<T> CreateANode<T>(INodeRepository<T> repo, long rootId,
            string name = "Deneme", long? parentId = null) {
            GeneralPurposeTreeNode<T> insertedNode;
            long idBeforeInserted;
            CreateANode<T>(repo, out insertedNode, out idBeforeInserted, rootId, name, parentId);
            return insertedNode;
        }
        #endregion
        #endregion
        #endregion
    }
}
