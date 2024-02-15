client = network_create_socket(0)
network_set_config(network_config_connect_timeout, 1000)
server = network_connect_raw(client, "127.0.0.1", 1333)
global.Server = server
var message = "StoneShard connect successfully."
SendMsg("HOK", "OnGameStart", true)