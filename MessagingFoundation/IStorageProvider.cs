namespace AdvantestMessagingFoundation
{
    public interface IStorageProvider
    {
        /// <summary>
        /// this can be implemented in inheriting class.
        /// </summary>
        /// <param name="data"></param>
        void StoreData(string data);
    }
}
