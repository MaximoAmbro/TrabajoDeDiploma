SET NOCOUNT ON;
-- Comparación de la instalación de desarrollo con la base creada por los scripts y las pruebas.
SELECT t.name Tabla,c.name Columna,ty.name Tipo,c.max_length Longitud,c.precision Precision,c.scale Escala,c.is_nullable Nulo,c.is_identity Identidad,dc.definition Predeterminado
INTO #Desarrollo
FROM BddEventAura.sys.tables t JOIN BddEventAura.sys.columns c ON t.object_id=c.object_id
JOIN BddEventAura.sys.types ty ON c.user_type_id=ty.user_type_id
LEFT JOIN BddEventAura.sys.default_constraints dc ON c.default_object_id=dc.object_id;
SELECT t.name Tabla,c.name Columna,ty.name Tipo,c.max_length Longitud,c.precision Precision,c.scale Escala,c.is_nullable Nulo,c.is_identity Identidad,dc.definition Predeterminado
INTO #Referencia
FROM BddEventAura_Pruebas.sys.tables t JOIN BddEventAura_Pruebas.sys.columns c ON t.object_id=c.object_id
JOIN BddEventAura_Pruebas.sys.types ty ON c.user_type_id=ty.user_type_id
LEFT JOIN BddEventAura_Pruebas.sys.default_constraints dc ON c.default_object_id=dc.object_id;
SELECT 'Columnas solo/diferentes en desarrollo' Resultado,COUNT(*) Diferencias FROM (SELECT * FROM #Desarrollo EXCEPT SELECT * FROM #Referencia) d;
SELECT 'Columnas solo/diferentes en referencia' Resultado,COUNT(*) Diferencias FROM (SELECT * FROM #Referencia EXCEPT SELECT * FROM #Desarrollo) d;
SELECT t.name Tabla,i.is_unique Unico,i.is_primary_key PK,i.type_desc Tipo,i.filter_definition Filtro,c.name Columna,ic.key_ordinal Orden,ic.is_included_column Incluida
INTO #IndicesDev FROM BddEventAura.sys.tables t JOIN BddEventAura.sys.indexes i ON t.object_id=i.object_id JOIN BddEventAura.sys.index_columns ic ON i.object_id=ic.object_id AND i.index_id=ic.index_id JOIN BddEventAura.sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id;
SELECT t.name Tabla,i.is_unique Unico,i.is_primary_key PK,i.type_desc Tipo,i.filter_definition Filtro,c.name Columna,ic.key_ordinal Orden,ic.is_included_column Incluida
INTO #IndicesRef FROM BddEventAura_Pruebas.sys.tables t JOIN BddEventAura_Pruebas.sys.indexes i ON t.object_id=i.object_id JOIN BddEventAura_Pruebas.sys.index_columns ic ON i.object_id=ic.object_id AND i.index_id=ic.index_id JOIN BddEventAura_Pruebas.sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id;
SELECT 'Indices diferentes dev/ref' Resultado,COUNT(*) Diferencias FROM (SELECT * FROM #IndicesDev EXCEPT SELECT * FROM #IndicesRef) d;
SELECT 'Indices diferentes ref/dev' Resultado,COUNT(*) Diferencias FROM (SELECT * FROM #IndicesRef EXCEPT SELECT * FROM #IndicesDev) d;
SELECT t.name Tabla,c.name Columna,r.name Referencia,rc.name ColumnaRef,fk.delete_referential_action Borrado,fk.update_referential_action Actualizado,fk.is_disabled Deshabilitada,fk.is_not_trusted NoConfiable
INTO #FkDev FROM BddEventAura.sys.foreign_keys fk JOIN BddEventAura.sys.foreign_key_columns fc ON fk.object_id=fc.constraint_object_id JOIN BddEventAura.sys.tables t ON t.object_id=fc.parent_object_id JOIN BddEventAura.sys.columns c ON c.object_id=t.object_id AND c.column_id=fc.parent_column_id JOIN BddEventAura.sys.tables r ON r.object_id=fc.referenced_object_id JOIN BddEventAura.sys.columns rc ON rc.object_id=r.object_id AND rc.column_id=fc.referenced_column_id;
SELECT t.name Tabla,c.name Columna,r.name Referencia,rc.name ColumnaRef,fk.delete_referential_action Borrado,fk.update_referential_action Actualizado,fk.is_disabled Deshabilitada,fk.is_not_trusted NoConfiable
INTO #FkRef FROM BddEventAura_Pruebas.sys.foreign_keys fk JOIN BddEventAura_Pruebas.sys.foreign_key_columns fc ON fk.object_id=fc.constraint_object_id JOIN BddEventAura_Pruebas.sys.tables t ON t.object_id=fc.parent_object_id JOIN BddEventAura_Pruebas.sys.columns c ON c.object_id=t.object_id AND c.column_id=fc.parent_column_id JOIN BddEventAura_Pruebas.sys.tables r ON r.object_id=fc.referenced_object_id JOIN BddEventAura_Pruebas.sys.columns rc ON rc.object_id=r.object_id AND rc.column_id=fc.referenced_column_id;
SELECT 'FK diferentes dev/ref' Resultado,COUNT(*) Diferencias FROM (SELECT * FROM #FkDev EXCEPT SELECT * FROM #FkRef) d;
SELECT 'FK diferentes ref/dev' Resultado,COUNT(*) Diferencias FROM (SELECT * FROM #FkRef EXCEPT SELECT * FROM #FkDev) d;
SELECT 'Integridad relaciones heredadas' Resultado;
SELECT 'Locales sin propietario' Caso,COUNT(*) Cantidad FROM Locales l LEFT JOIN Usuario u ON u.ID=l.PropietarioID WHERE u.ID IS NULL
UNION ALL SELECT 'Entradas sin usuario',COUNT(*) FROM Entradas e LEFT JOIN Usuario u ON u.ID=e.ID_Usuario WHERE u.ID IS NULL
UNION ALL SELECT 'Correos duplicados',COUNT(*) FROM (SELECT Mail FROM Usuario GROUP BY Mail HAVING COUNT(*)>1) x
UNION ALL SELECT 'Sector y entrada de distintos eventos',COUNT(*) FROM Entradas e JOIN Sectores s ON s.ID=e.ID_Sector WHERE e.ID_Evento<>s.EventoID;
DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;
