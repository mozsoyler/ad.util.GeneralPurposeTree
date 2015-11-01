using ad.util.GeneralPurposeTreeRepos;
using Insight.Database;
using Insight.Database.Schema;
using Ninject;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ad.util {
    public class GeneralPurposeTree<NT> {
        #region constants
        private const string DbSchemaName = "ad.util.GeneralPurposeTree";
        #endregion

        #region fields
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IList<GeneralPurposeTreeNode<NT>> _hive;
        #endregion

        #region constructors
        static GeneralPurposeTree() {
            CreateOrModifyDbSchema();
        }
        #endregion

        #region properties
        #region public properties
        #region public static properties
        [Inject]
        public IRootRepository<NT> Repository { get; set; }
        #endregion

        public long Id { get; set; }
        public string Name { get; set; }
        public string Initiator { get; set; }
        public IList<GeneralPurposeTreeNode<NT>> Hive {
            get {
                if (_hive == null) {
                    _hive = GetHive();
                }
                return _hive;
            }
            set { _hive = value; }
        }
        #endregion
        #endregion

        #region methods
        #region public methods
        #region public static methods

        #region private methods
        private IList<GeneralPurposeTreeNode<NT>> GetHive() {
            var nodes = Repository.GetTreeUnderRoot(Id);
            return TreeUtils.BuildSubtree(nodes);
            //var stack = new Stack<GeneralPurposeTreeNode<NT>>();
            //Func<GeneralPurposeTreeNode<NT>> getParent = () => stack.Count == 0 ? null : stack.Peek();
            //var result = new List<GeneralPurposeTreeNode<NT>>();
            //foreach (var node in nodes) {
            //    var parent = getParent();
            //    try {
            //        while (parent != null && parent.Id != node.ParentId) {
            //            stack.Pop();
            //            parent = getParent();
            //        }
            //        if (parent == null)
            //            continue;
            //        if (parent.Childs == null)
            //            parent.Childs = new List<GeneralPurposeTreeNode<NT>>();
            //        parent.Childs.Add(node);
            //        node.Parent = parent;
            //    }
            //    finally {
            //        if (parent == null) {
            //            result.Add(node);
            //            node.Parent = null;
            //        }
            //        stack.Push(node);
            //    }
            //}
            //return result;
        }

        #region private static methods
        private static void CreateOrModifyDbSchema() {
            var connStr = ConfigurationManager.ConnectionStrings["ad.util.GeneralPurposeTree"].ConnectionString;
            if (!SchemaInstaller.DatabaseExists(connStr)) {
                _logger.Info("ad.util.GeneralPurposeTree is about to create a database.");
                SchemaInstaller.CreateDatabase(connStr);
                _logger.Info("ad.util.GeneralPurposeTree has just created a database.");
            }
            using (var conn = new SqlConnection(connStr)) {
                conn.Open();
                var installer = new SchemaInstaller(conn);
                installer.CreatingObject += SchemaInstaller_CreatingObject;
                installer.CreatedObject += SchemaInstaller_CreatedObject;
                installer.DroppingObject += SchemaInstaller_DroppingObject;
                installer.DropFailed += SchemaInstaller_DropFailed;
                _logger.Info("Database modification starting for database '{0}' and schema '{1}'.", conn.Database, DbSchemaName);
                var saveCultureInfo = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                try {
                    var schema = new SchemaObjectCollection();
                    schema.Load(System.Reflection.Assembly.GetExecutingAssembly());
                    installer.Install(DbSchemaName, schema);
                }
                finally {
                    Thread.CurrentThread.CurrentCulture = saveCultureInfo;
                }
                _logger.Info("Database modification ended for database '{0}' and schema '{1}'.", conn.Database, DbSchemaName);
            }
        }
        #endregion
        #endregion

        #region eventhandlers
        static void SchemaInstaller_DropFailed(object sender, SchemaEventArgs e) {
            _logger.Info(e.Exception,
                "Dropping DB object failed! See succeeding log records: schema name={0}, object name={1} ",
                e.SchemaObject == null ? "<SchemaObject is null>" : e.SchemaObject.Name, e.ObjectName);
        }

        static void SchemaInstaller_DroppingObject(object sender, SchemaEventArgs e) {
            _logger.Info("Dropping DB object: schema name={0}, object name={1}, ",
                e.SchemaObject == null ? "<SchemaObject is null>" : e.SchemaObject.Name, e.ObjectName);
        }

        static void SchemaInstaller_CreatedObject(object sender, SchemaEventArgs e) {
            _logger.Info("Created DB object: schema name={0}, object name={1}, ",
                e.SchemaObject == null ? "<SchemaObject is null>" : e.SchemaObject.Name, e.ObjectName);
        }

        static void SchemaInstaller_CreatingObject(object sender, SchemaEventArgs e) {
            _logger.Info("Creating DB object: schema name={0}, object name={1}, ",
                e.SchemaObject == null ? "<SchemaObject is null>" : e.SchemaObject.Name, e.ObjectName);
        }
        #endregion
        #endregion
        #endregion
        #endregion
    }
}
