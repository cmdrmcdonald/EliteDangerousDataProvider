using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace Eddi
{
    /// <summary>
    /// Persistent state
    /// </summary>
    public class PState : INotifyCollectionChanged
    {
        private static PState instance;

        private static readonly object instanceLock = new object();
        public static PState Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            Logging.Debug("No state instance: creating one");
                            instance = new PState();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>Event raised when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly SynchronizationContext context;
        private ConcurrentDictionary<string, object> data;

        public ICollection<string> Keys
        {
            get { return data.Keys; }
        }

        /// <summary>
        /// Notifies observers of CollectionChanged of an update to the state.
        /// </summary>
        private void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            if (collectionHandler != null)
            {
                context.Post(s =>
                {
                    collectionHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }, null);
            }
        }

        public void SetString(string key, string value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                if (data.TryAdd(key, value))
                {
                    writeData();
                    NotifyObserversOfChange();
                }
            }
        }

        public string GetString(string key)
        {
            object value;
            string result = null;
            if (data.TryGetValue(key, out value))
            {
                if (value.GetType() == typeof(string))
                {
                    result = (string)value;
                }
            }

            return result;
        }

        public void SetNumber(string key, decimal? value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                if (data.TryAdd(key, value))
                {
                    writeData();
                    NotifyObserversOfChange();
                }
            }
        }

        public decimal? GetNumber(string key)
        {
            object value;
            decimal? result = null;
            if (data.TryGetValue(key, out value))
            {
                if (value.GetType() == typeof(int))
                {
                    result = (decimal)(int)value;
                }
                else if (value.GetType() == typeof(float))
                {
                    result = (decimal)(float)value;
                }
                else if (value.GetType() == typeof(double))
                {
                    result = (decimal)(double)value;
                }
                else if (value.GetType() == typeof(decimal))
                {
                    result = (decimal)value;
                }
            }

            return result;
        }

        public void SetFlag(string key, bool? value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                if (data.TryAdd(key, value))
                {
                    writeData();
                    NotifyObserversOfChange();
                }
            }
        }

        public bool? GetFlag(string key)
        {
            object value;
            bool? result = null;
            if (data.TryGetValue(key, out value))
            {
                if (value.GetType() == typeof(bool))
                {
                    result = (bool)value;
                }
            }

            return result;
        }

        public object Get(string key)
        {
            object value;
            data.TryGetValue(key, out value);
            return value;
        }

        public void RemoveKey(string key)
        {
            object old;
            if (data.TryRemove(key, out old))
            {
                writeData();
                NotifyObserversOfChange();
            }
        }

        public PState()
        {
            context = AsyncOperationManager.SynchronizationContext;

            // Init state if it doesn't exist
            initData();

            // Fetch the state
            readData();
        }

        private void initData()
        {
            try
            {
                if (!File.Exists(Constants.DATA_DIR + @"\state.json"))
                {
                    File.WriteAllText(Constants.DATA_DIR + @"\state.json", @"{}");
                }
            }
            catch (Exception ex)
            {
                Logging.Warn("Failed to initialise state", ex);
            }
        }

        private void writeData()
        {
            try
            {
                string stateJson = JsonConvert.SerializeObject(data);
                File.WriteAllText(Constants.DATA_DIR + @"\state.json", stateJson);
            }
            catch (Exception ex)
            {
                Logging.Warn("Failed to write state", ex);
            }
        }

        private void readData()
        {
            try
            {
                string stateJson = File.ReadAllText(Constants.DATA_DIR + @"\state.json");
                data = JsonConvert.DeserializeObject<ConcurrentDictionary<string, object>>(stateJson);
            }
            catch (Exception ex)
            {
                Logging.Warn("Failed to read state", ex);
            }
        }
    }
}
