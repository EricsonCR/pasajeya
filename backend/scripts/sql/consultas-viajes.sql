--Consultar tablas en general
select * from boletos;
select * from viajes where id = 627;
select * from buses where id = 6;
select * from rutas where id = 15;
select * from ciudades where id = 1 or id = 2;

--Regenerar viajes del seed: borrar en este orden por Restrict.
--DELETE FROM Boletos;
--DELETE FROM Viajes;

--consultar viajes por ruta y fecha salida
select * from viajes where rutaId = 15 and salida >= '2026-10-07';

--consultar boletos activos por origen, destino y fecha salida
select bl.Id, bl.Estado, bs.Id, bs.Capacidad, v.Id, v.Salida, v.Llegada from Boletos bl
inner join Viajes v on v.Id = bl.ViajeId
inner join Buses bs on bs.Id = v.BusId
inner join Rutas r on r.Id = v.RutaId
where r.OrigenId = 6 and r.DestinoId = 3 and v.Salida >= '2026-10-05';
