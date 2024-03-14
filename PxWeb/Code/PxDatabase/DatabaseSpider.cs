using System.Linq;

using PxWeb.Code.BackgroundWorker;
using PxWeb.Code.PxDatabase;

namespace PXWeb.Database
{

    public class DatabaseMessage
    {
        public string Message { get; set; } = string.Empty;
        public BuilderMessageType MessageType { get; set; }

        public enum BuilderMessageType
        {
            Information,
            Warning,
            Error
        }
    }

    /// Callback method to logg messages from the builder
    /// </summary>
    /// <param name="msg">Message to log</param>
    public delegate void DatabaseLogger(DatabaseMessage msg);

    /// <summary>
    /// DatabaseSpider iterates through a file database and ...TODO
    /// </summary>
    public class DatabaseSpider
    {
        public DatabaseSpider()
        {
            logger = new DatabaseLogger(LogMessage);
        }

        private readonly List<IItemHandler> _handlers = new List<IItemHandler>();
        public List<IItemHandler> Handles { get { return _handlers; } }

        private readonly List<IDatabaseBuilder> _builders = new List<IDatabaseBuilder>();
        public List<IDatabaseBuilder> Builders { get { return _builders; } }

        private readonly List<DatabaseMessage> _messages = new List<DatabaseMessage>();
        public List<DatabaseMessage> Messages { get { return _messages; } }

        private readonly DatabaseLogger logger;
        private bool _stateLogging = false;
        private IControllerState? _responseState;
        private void LogMessage(DatabaseMessage msg)
        {
            Messages.Add(msg);
            if (_stateLogging)
            {
                if (_responseState != null)
                {
                    _responseState.AddEvent(new Event(msg.MessageType.ToString(), msg.Message));
                }
            }
        }

        /// <summary>
        /// Activate logging to a ResponseState in order to track progress of async tasks
        /// </summary>
        /// <param name="responseState">The state to be logged to</param>
        public void ActivateStateLogging(IControllerState responseState)
        {
            _responseState = responseState;
            _stateLogging = true;
        }

        /// <summary>
        /// Deactivate logging to a ResponseState
        /// </summary>
        public void DeactivateStateLogging()
        {
            _stateLogging = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPath">The root path of the database</param>
        public void Search(string startPath)
        {
            //Sort handlers after priority
            Handles.OrderBy(x => x.Priority);

            foreach (var builder in Builders)
            {
                builder.BeginBuild(startPath, logger);
            }


            try
            {
                SearchRecursive(startPath);
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("Cannot search {0}. {1}", startPath, e.Message);


                logger(new DatabaseMessage()
                {
                    MessageType = DatabaseMessage.BuilderMessageType.Error,
                    Message = errorMessage
                });
            }

            foreach (var builder in Builders)
            {
                builder.EndBuild(startPath);
            }
        }

        /// <summary>
        /// Searches recursively the file database
        /// </summary>
        /// <param name="path">The path to search</param>
        private void SearchRecursive(string path)
        {
            //allocates a new menu
            SignalStartNewLevel(path);

            string[] filesInDir = System.IO.Directory.GetFiles(path);
            //Ensuring the languageless alias file is applied first:
            Array.Sort(filesInDir, new AliasTxtFirstComparer());
            foreach (var item in filesInDir)
            {
                IItemHandler? handler = GetHandler(item);
                if (handler != null)
                {
                    object? obj = handler.Handle(item, logger);
                    if (obj != null)
                    {
                        SignalNewItem(obj, item);
                    }
                    //Should there be an else logg here as well?
                }
                else
                {
                    //TODO LOGG
                }
            }

            foreach (var item in System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly))
            {
                SearchRecursive(item);
            }

            SignalStopNewLevel(path);
        }

        private IItemHandler? GetHandler(string path)
        {
            foreach (var item in Handles)
            {
                if (item.CanHandle(path)) return item;
            }
            return null;
        }

        private void SignalStartNewLevel(string path)
        {
            foreach (var item in Builders)
            {
                item.BeginNewLevel(path);
            }
        }

        private void SignalStopNewLevel(string path)
        {
            foreach (var item in Builders)
            {
                item.EndNewLevel(path);
            }
        }

        private void SignalNewItem(object newItem, string path)
        {
            foreach (var item in Builders)
            {
                item.NewItem(newItem, path);
            }
        }
    }
}
