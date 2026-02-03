Config = {}

-- Menu Settings
Config.MenuKey = 'F1' -- Key to open the menu
Config.MenuTitle = 'CBPS Menu'

-- Permission Settings
Config.AdminOnly = false -- Set to true to restrict menu to admins only
Config.Admins = {} -- List of admin identifiers

-- Theme Settings
Config.DefaultTheme = 'blue' -- Default theme: blue, red, green, purple, orange, yellow, pink, dark, light
Config.AllowThemeChange = true -- Allow players to change theme

Config.Themes = {
    blue = {
        name = 'Blue',
        banner = {r = 0, g = 120, b = 215, a = 255},
        highlight = {r = 0, g = 120, b = 215, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    red = {
        name = 'Red',
        banner = {r = 220, g = 20, b = 60, a = 255},
        highlight = {r = 220, g = 20, b = 60, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    green = {
        name = 'Green',
        banner = {r = 34, g = 139, b = 34, a = 255},
        highlight = {r = 34, g = 139, b = 34, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    purple = {
        name = 'Purple',
        banner = {r = 138, g = 43, b = 226, a = 255},
        highlight = {r = 138, g = 43, b = 226, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    orange = {
        name = 'Orange',
        banner = {r = 255, g = 140, b = 0, a = 255},
        highlight = {r = 255, g = 140, b = 0, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    yellow = {
        name = 'Yellow',
        banner = {r = 255, g = 215, b = 0, a = 255},
        highlight = {r = 255, g = 215, b = 0, a = 255},
        textColor = {r = 0, g = 0, b = 0, a = 255}
    },
    pink = {
        name = 'Pink',
        banner = {r = 255, g = 20, b = 147, a = 255},
        highlight = {r = 255, g = 20, b = 147, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    dark = {
        name = 'Dark',
        banner = {r = 30, g = 30, b = 30, a = 255},
        highlight = {r = 60, g = 60, b = 60, a = 255},
        textColor = {r = 255, g = 255, b = 255, a = 255}
    },
    light = {
        name = 'Light',
        banner = {r = 240, g = 240, b = 240, a = 255},
        highlight = {r = 200, g = 200, b = 200, a = 255},
        textColor = {r = 0, g = 0, b = 0, a = 255}
    }
}

-- VMenu Settings
Config.VMenu = {
    PlayerOptions = true,
    VehicleSpawner = true,
    VehicleOptions = true,
    WeaponOptions = true,
    TimeWeatherOptions = true,
    WorldOptions = true,
    MiscSettings = true
}

-- Voice Settings (pma-voice integration)
Config.Voice = {
    Enabled = true,
    DefaultRange = 5.0, -- Default voice range in meters
    Ranges = {3.0, 5.0, 10.0, 15.0, 20.0, 30.0}, -- Available voice ranges
    ShowUI = true
}

-- Radio Settings (pma-radio integration)
Config.Radio = {
    Enabled = true,
    MaxFrequency = 999.9,
    MinFrequency = 1.0
}

-- Team Management Settings
Config.Team = {
    Enabled = true,
    MaxTeamSize = 8,
    TeamColors = {
        {r = 255, g = 0, b = 0},    -- Red
        {r = 0, g = 255, b = 0},    -- Green
        {r = 0, g = 0, b = 255},    -- Blue
        {r = 255, g = 255, b = 0},  -- Yellow
        {r = 255, g = 0, b = 255},  -- Magenta
        {r = 0, g = 255, b = 255}   -- Cyan
    }
}

-- Race Settings
Config.Race = {
    Enabled = true,
    MaxCheckpoints = 20,
    CheckpointRadius = 10.0,
    CountdownTime = 5, -- seconds
    ShowLeaderboard = true
}

-- Vehicle Spawner Categories
Config.VehicleCategories = {
    {name = 'Super', vehicles = {'adder', 'autarch', 'banshee2', 'bullet', 'cheetah', 'cyclone', 'entityxf', 'fmj', 'gp1', 'infernus', 'nero', 'osiris', 'penetrator', 'reaper', 't20', 'taipan', 'tempesta', 'turismor', 'tyrus', 'vacca', 'visione', 'voltic', 'xa21', 'zentorno'}},
    {name = 'Sports', vehicles = {'alpha', 'banshee', 'bestiagts', 'blista2', 'buffalo', 'buffalo2', 'buffalo3', 'carbonizzare', 'comet2', 'comet3', 'coquette', 'elegy', 'elegy2', 'feltzer2', 'furoregt', 'fusilade', 'futo', 'jester', 'jester2', 'khamelion', 'kuruma', 'lynx', 'massacro', 'neon', 'ninef', 'ninef2', 'pariah', 'penumbra', 'raiden', 'rapidgt', 'rapidgt2', 'revolter', 'ruston', 'schafter3', 'schafter4', 'schafter5', 'schwarzer', 'sentinel3', 'seven70', 'specter', 'specter2', 'streiter', 'sultan', 'surano', 'tampa2', 'tropos', 'verlierer2'}},
    {name = 'SUVs', vehicles = {'baller', 'baller2', 'baller3', 'baller4', 'baller5', 'baller6', 'bjxl', 'cavalcade', 'cavalcade2', 'contender', 'dubsta', 'dubsta2', 'fq2', 'granger', 'gresley', 'habanero', 'huntley', 'landstalker', 'mesa', 'patriot', 'radi', 'rocoto', 'seminole', 'serrano', 'xls', 'xls2'}},
    {name = 'Sedans', vehicles = {'asea', 'asterope', 'cog55', 'cog552', 'cognoscenti', 'cognoscenti2', 'emperor', 'emperor2', 'emperor3', 'fugitive', 'glendale', 'ingot', 'intruder', 'limo2', 'premier', 'primo', 'primo2', 'regina', 'schafter2', 'stanier', 'stratum', 'stretch', 'surge', 'tailgater', 'warrener', 'washington'}},
    {name = 'Motorcycles', vehicles = {'akuma', 'avarus', 'bagger', 'bati', 'bati2', 'bf400', 'carbonrs', 'chimera', 'cliffhanger', 'daemon', 'daemon2', 'defiler', 'diablous', 'diablous2', 'double', 'enduro', 'esskey', 'faggio', 'faggio2', 'faggio3', 'gargoyle', 'hakuchou', 'hakuchou2', 'hexer', 'innovation', 'lectro', 'manchez', 'nemesis', 'nightblade', 'pcj', 'ratbike', 'ruffian', 'sanchez', 'sanchez2', 'sanctus', 'shotaro', 'sovereign', 'thrust', 'vader', 'vindicator', 'vortex', 'wolfsbane', 'zombiea', 'zombieb'}},
    {name = 'Emergency', vehicles = {'ambulance', 'fbi', 'fbi2', 'firetruk', 'lguard', 'pbus', 'police', 'police2', 'police3', 'police4', 'policeb', 'policeold1', 'policeold2', 'policet', 'pranger', 'predator', 'riot', 'sheriff', 'sheriff2'}},
}

-- Weapon Categories
Config.WeaponCategories = {
    {name = 'Melee', weapons = {'WEAPON_KNIFE', 'WEAPON_NIGHTSTICK', 'WEAPON_HAMMER', 'WEAPON_BAT', 'WEAPON_GOLFCLUB', 'WEAPON_CROWBAR', 'WEAPON_BOTTLE', 'WEAPON_DAGGER', 'WEAPON_HATCHET', 'WEAPON_KNUCKLE', 'WEAPON_MACHETE', 'WEAPON_FLASHLIGHT', 'WEAPON_SWITCHBLADE', 'WEAPON_POOLCUE', 'WEAPON_WRENCH'}},
    {name = 'Handguns', weapons = {'WEAPON_PISTOL', 'WEAPON_COMBATPISTOL', 'WEAPON_APPISTOL', 'WEAPON_PISTOL50', 'WEAPON_SNSPISTOL', 'WEAPON_HEAVYPISTOL', 'WEAPON_VINTAGEPISTOL', 'WEAPON_MARKSMANPISTOL', 'WEAPON_REVOLVER', 'WEAPON_DOUBLEACTION'}},
    {name = 'Submachine Guns', weapons = {'WEAPON_MICROSMG', 'WEAPON_SMG', 'WEAPON_ASSAULTSMG', 'WEAPON_COMBATPDW', 'WEAPON_MACHINEPISTOL', 'WEAPON_MINISMG', 'WEAPON_GUSENBERG'}},
    {name = 'Shotguns', weapons = {'WEAPON_PUMPSHOTGUN', 'WEAPON_SAWNOFFSHOTGUN', 'WEAPON_ASSAULTSHOTGUN', 'WEAPON_BULLPUPSHOTGUN', 'WEAPON_MUSKET', 'WEAPON_HEAVYSHOTGUN', 'WEAPON_DBSHOTGUN', 'WEAPON_AUTOSHOTGUN'}},
    {name = 'Assault Rifles', weapons = {'WEAPON_ASSAULTRIFLE', 'WEAPON_CARBINERIFLE', 'WEAPON_ADVANCEDRIFLE', 'WEAPON_SPECIALCARBINE', 'WEAPON_BULLPUPRIFLE', 'WEAPON_COMPACTRIFLE'}},
    {name = 'Sniper Rifles', weapons = {'WEAPON_SNIPERRIFLE', 'WEAPON_HEAVYSNIPER', 'WEAPON_MARKSMANRIFLE'}},
    {name = 'Heavy Weapons', weapons = {'WEAPON_RPG', 'WEAPON_GRENADELAUNCHER', 'WEAPON_MINIGUN', 'WEAPON_FIREWORK', 'WEAPON_RAILGUN', 'WEAPON_HOMINGLAUNCHER', 'WEAPON_COMPACTLAUNCHER'}},
}
