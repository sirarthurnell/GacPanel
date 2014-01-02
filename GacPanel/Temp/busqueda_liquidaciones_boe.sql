select * from a153sf liquidacion where liquidacion.fdrid in (

	select liquidacion.fdrid from a162sf historial inner join a153sf liquidacion
	on historial.fld1 = liquidacion.fdrid inner join a118sf expediente
	on liquidacion.fld1 = expediente.fld1
	where historial.fld3 in('VBOE', 'DEV')
		and historial.fld5 > '01/01/2013'
		and expediente.fld1 like 'A %'

)