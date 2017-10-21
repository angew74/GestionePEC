
create function [ssma_oracle].[get_active_spid]()
returns int
as begin
declare @current_spid smallint,@login_time datetime
select @login_time=login_time from sys.dm_exec_sessions where session_id=@@spid
select @current_spid=cast(v_value as int) from ssma_oracle.db_storage with (READCOMMITTEDLOCK)
where login_time=@login_time and spid=@@spid and name='$spid$'
and exists (select * from ssma_oracle.db_storage with (READCOMMITTEDLOCK) where 
login_time=@login_time and spid=@@spid and name='$login_time$')
return isnull(@current_spid,@@spid)
end

