namespace FrameWork
{
    public class DefinitionBase : IDefinitionDataBase
    {
        public string key = "";

        public string GetKey()
        {
            return key;
        }

        public virtual void Initialize()
        {
            
        }
    }
}