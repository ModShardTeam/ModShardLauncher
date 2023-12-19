if (global.gamespeed == global.default_gamespeed)
{
    global.gamespeed = global.accelerated_gamespeed
    scr_actionsLogUpdate("F1: AccelerateSpeed")
}
else
{
    global.gamespeed = global.default_gamespeed
    scr_actionsLogUpdate("F1: DefaultSpeed")
}
game_set_speed(global.gamespeed, gamespeed_fps)