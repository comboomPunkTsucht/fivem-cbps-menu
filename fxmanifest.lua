fx_version 'cerulean'
game 'gta5'

author 'comboom.sucht'
description 'A vMenu-style FiveM menu using LemonUI with Nord theme, pma-voice/radio control, team management, and race features'
version '1.0.0'

-- C# Menu using LemonUI (main entry point)
client_scripts {
    'CBPSMenu.net.dll'
}

server_scripts {
    'CBPSMenu.Server.net.dll'
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
