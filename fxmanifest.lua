fx_version 'cerulean'
game 'gta5'

author 'comboom.sucht'
description 'A vMenu-style FiveM menu using LemonUI with Nord theme, pma-voice/radio control, team management, and race features'
version '1.0.0'

-- C# Client script
client_scripts {
    'Client/CBPSMenu.net.dll'
}

-- C# Server script
server_scripts {
    'Server/CBPSMenu.Server.net.dll'
}

-- Required files
files {
    'lib/LemonUI.FiveM.dll',
    'lib/Newtonsoft.Json.dll',
    'config.json'
}

-- Required dependencies
dependencies {
    'pma-voice',
    'pma-radio'
}
