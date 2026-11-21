using System;
using Entities.Locations;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Легковесный дескриптор сущности на уровне L0 (спящая запись в реестре мира сессии).
    /// Хранится в оперативной памяти EntitiesManager._entitiesL0 (до 10 000 шт.)
    /// и сериализуется в единый файл сохранения сессии.
    /// Не создает объектов в сцене Unity и не занимает память под компоненты.
    /// </summary>
    [Serializable]
    public struct EntityL0
    {
        /// <summary>
        /// Уникальный строковый идентификатор сущности в рамках игровой сессии.
        /// </summary>
        public string EntityId;

        /// <summary>
        /// Идентификатор локации / пространства мира (см. LocationIds), в котором находится сущность.
        /// </summary>
        public string LocationId;

        /// <summary>
        /// Локальные координаты сущности внутри данной локации.
        /// </summary>
        public Vector3 LocalPosition;

        /// <summary>
        /// Радиус интереса/габариты сущности для пространственного стриминга (дистанция активации L0 -> L1).
        /// </summary>
        public float Radius;

        /// <summary>
        /// Ссылка на шаблон / префаб / Blueprint сущности (для генерации по шаблону).
        /// </summary>
        public string TemplateId;

        /// <summary>
        /// Идентификатор спавнера, породившего эту сущность (null для уникальных и запеченных в мире).
        /// </summary>
        public string SpawnerId;

        /// <summary>
        /// Признак персистентности:
        /// true — сюжетный персонаж, уникальный сундук, стационарный объект (сохраняется всегда);
        /// false — временный моб, снаряд, эффект (может быть уничтожен при очистке).
        /// </summary>
        public bool IsPersistent;

        /// <summary>
        /// Флаг прохождения первичной инициализации сущности (Setup).
        /// </summary>
        public bool IsConfigured;

        public EntityL0(
            string entityId,
            string locationId = LocationIds.Default,
            Vector3 localPosition = default,
            float radius = 100f,
            string templateId = null,
            string spawnerId = null,
            bool isPersistent = true,
            bool isConfigured = false)
        {
            EntityId = entityId;
            LocationId = string.IsNullOrEmpty(locationId) ? LocationIds.Default : locationId;
            LocalPosition = localPosition;
            Radius = radius > 0f ? radius : 100f;
            TemplateId = templateId;
            SpawnerId = spawnerId;
            IsPersistent = isPersistent;
            IsConfigured = isConfigured;
        }

        public override string ToString()
        {
            return $"[EntityL0: {EntityId} at {LocalPosition} (Loc: {LocationId}, R: {Radius})]";
        }
    }
}
