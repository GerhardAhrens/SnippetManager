//-----------------------------------------------------------------------
// <copyright file="TableColumnAttribute.cs" company="Lifeprojects.de">
//     Class: TableColumnAttribute
//     Copyright © Gerhard Ahrens, 2019
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>development@lifeprojects.de</email>
// <date>30.12.2019</date>
//
// <summary>Enum Class for TableSchemaTyp</summary>
//-----------------------------------------------------------------------

namespace System.Data.SQLite
{
    public enum TableSchemaTyp
    {
        None = 0,
        MetaDataCollections = 1,
        DataSourceInformation =2,
        DataTypes = 3,
        ReservedWords = 4,
        Catalogs = 5,
        Columns = 6,
        Indexes = 7,
        IndexColumns = 8,
        Tables = 9,
        Views = 10,
        ViewColumns = 11,
        ForeignKeys = 12,
        Triggers = 13
    }
}
