myid = 99999;
target_id = 99999;

function set_uid(x)
	myid = x;
end

function event_player_move(player)
	player_x = API_get_x(player);
	player_y = API_get_y(player);
	my_x = API_get_x(myid);
	my_y = API_get_y(myid);
	if (player_x == my_x) then
		if (player_y == my_y) then
			target_id = player;
		end
	end
end

function get_taget_id()
	return target_id;
end
