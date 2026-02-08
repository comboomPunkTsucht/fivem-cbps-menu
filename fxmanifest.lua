fx_version 'cerulean'
game 'gta5'

author 'comboom.sucht'
description 'A vMenu-style FiveM menu using LemonUI with Nord theme, pma-voice/radio control, team management, and race features'
version '1.0.0'

-- C# Client script
client_scripts {
    'CBPSMenu.net.dll'
}

-- C# Server script
server_scripts {
    'CBPSMenu.Server.net.dll'
}

-- Required files
files {
    'LemonUI.FiveM.dll',
    'Newtonsoft.Json.dll',
    'config.json'
}

-- Required dependencies
dependencies {
    'pma-voice',
    'pma-radio'
}
