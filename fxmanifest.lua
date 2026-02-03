fx_version 'cerulean'
game 'gta5'

author 'CBPS Menu'
description 'A vMenu-style FiveM menu using LemonUI with Nord theme, pma-voice/radio control, team management, and race features'
version '1.0.0'

-- C# Menu using LemonUI (main entry point)
client_scripts {
    'CBPSMenu.net.dll'
}

-- Lua support scripts (for features that work alongside the C# menu)
client_scripts {
    'config.lua',
    'client/voice.lua'
}

server_scripts {
    'config.lua',
    'server/main.lua',
    'server/settings.lua',
    'server/player.lua',
    'server/vehicle.lua',
    'server/weapons.lua',
    'server/voice.lua',
    'server/team.lua',
    'server/race.lua'
}

-- LemonUI library and Newtonsoft.Json for race system
files {
    'LemonUI.FiveM.dll',
    'Newtonsoft.Json.dll'
}

-- Required dependencies
dependencies {
    'pma-voice',
    'pma-radio'
}
