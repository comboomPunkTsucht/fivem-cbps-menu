fx_version 'cerulean'
game 'gta5'

author 'CBPS Menu'
description 'A comprehensive FiveM menu with VMenu functions, pma-voice/radio control, team management, and race features'
version '1.0.0'

client_scripts {
    '@lemonui/LemonUI.net.dll',
    'config.lua',
    'client/main.lua',
    'client/settings.lua',
    'client/menu.lua',
    'client/player.lua',
    'client/vehicle.lua',
    'client/weapons.lua',
    'client/voice.lua',
    'client/team.lua',
    'client/race.lua',
    'client/character.lua'
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

dependencies {
    'lemonui',
    'pma-voice',
    'pma-radio'
}
