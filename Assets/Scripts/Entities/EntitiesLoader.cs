namespace Entities
{
    /// <summary>
    /// Заглушка загрузчика сущностей для сессии (GameSession).
    /// Зарезервирована для связи с редакторскими пайплайнами запекания мира.
    /// </summary>
    public class EntitiesLoader
    {
        public void RegisterMissingEntities(EntitiesManager entitiesManager) { }
        public void InitEntitiesRuntime(EntitiesManager entitiesManager) { }
    }
}
