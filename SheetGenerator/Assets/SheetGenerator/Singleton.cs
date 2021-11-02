using UnityEngine;

namespace FrameWork
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static GameObject _gameObject;
        protected static T _instance;
        public static bool IsNull => _instance == null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>(true);

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this as T;
        }

        private void OnDestroy()
        {
            _gameObject = null;
            _instance = null;

            Destroy();
        }

        public virtual void Destroy()
        {
            if (_instance != null) _instance = null;

            if (_gameObject != null)
            {
                Destroy(_gameObject);
                _gameObject = null;
            }
        }
    }
}