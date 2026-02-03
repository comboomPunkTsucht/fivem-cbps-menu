-- Server Settings Management

local serverDefaults = {
    -- Theme preferences
    theme = {
        current = 'blue',
        customThemes = {}
    },
    -- Character preferences
    character = {
        savedCharacters = {},
        defaultCharacter = nil,
        autoLoadDefault = true
    },
    -- Menu preferences
    menu = {
        lastOpenedMenu = 'main',
        rememberPosition = true
    },
    -- Voice settings
    voice = {
        defaultRange = 5.0,
        currentRangeIndex = 2,
        autoMute = false
    },
    -- Radio settings
    radio = {
        lastFrequency = 0.0,
        savedFrequencies = {},
        defaultFrequency = nil
    },
    -- Player options
    player = {
        godMode = false,
        invisible = false,
        noclip = false,
        superJump = false,
        fastRun = false
    },
    -- Vehicle preferences
    vehicle = {
        lastSpawned = nil,
        favoriteVehicles = {},
        autoRepair = false,
        autoUpgrade = false
    },
    -- Weapon preferences
    weapon = {
        favoriteWeapons = {},
        infiniteAmmo = false,
        noReload = false
    },
    -- World preferences
    world = {
        lastWeather = 'CLEAR',
        lastTime = {hour = 12, minute = 0},
        syncWithServer = true
    },
    -- Race preferences
    race = {
        favoriteRaces = {},
        autoJoin = false
    },
    -- Team preferences
    team = {
        autoAcceptInvites = false,
        preferredColor = nil
    },
    -- UI preferences
    ui = {
        notifications = true,
        sounds = true,
        showTips = true
    }
}

local playerSettings = {} -- Store player-specific settings

-- Load server defaults from file
function LoadServerDefaults()
    local data = LoadResourceFile(GetCurrentResourceName(), 'server_defaults.json')
    if data then
        local loaded = json.decode(data)
        if loaded then
            serverDefaults = loaded
            print('[CBPS Menu] Server defaults loaded from file')
        end
    else
        -- Save current defaults to file
        SaveServerDefaults()
    end
end

-- Save server defaults to file
function SaveServerDefaults()
    local data = json.encode(serverDefaults, {indent = true})
    SaveResourceFile(GetCurrentResourceName(), 'server_defaults.json', data, -1)
    print('[CBPS Menu] Server defaults saved to file')
end

-- Get player settings
function GetPlayerSettings(playerId)
    local identifier = GetPlayerIdentifier(playerId, 0)
    if not identifier then return nil end
    
    return playerSettings[identifier]
end

-- Save player settings
function SavePlayerSettings(playerId, settings)
    local identifier = GetPlayerIdentifier(playerId, 0)
    if not identifier then return false end
    
    playerSettings[identifier] = settings
    
    -- Save to database or file (for now, just memory)
    -- TODO: Add database integration
    
    return true
end

-- Request server defaults
RegisterNetEvent('cbps:requestServerDefaults')
AddEventHandler('cbps:requestServerDefaults', function()
    local playerId = source
    TriggerClientEvent('cbps:receiveServerDefaults', playerId, serverDefaults)
end)

-- Player setting changed
RegisterNetEvent('cbps:settingChanged')
AddEventHandler('cbps:settingChanged', function(category, key, value)
    local playerId = source
    local identifier = GetPlayerIdentifier(playerId, 0)
    
    if not identifier then return end
    
    if not playerSettings[identifier] then
        playerSettings[identifier] = {}
    end
    
    if not playerSettings[identifier][category] then
        playerSettings[identifier][category] = {}
    end
    
    playerSettings[identifier][category][key] = value
    
    print('[CBPS Menu] Setting changed for ' .. GetPlayerName(playerId) .. ': ' .. category .. '.' .. key)
end)

-- Admin command to update server defaults
RegisterCommand('cbps_set_default', function(source, args, rawCommand)
    if source == 0 or IsPlayerAdmin(source) then
        if #args < 3 then
            print('[CBPS Menu] Usage: cbps_set_default <category> <key> <value>')
            return
        end
        
        local category = args[1]
        local key = args[2]
        local value = args[3]
        
        -- Convert value to appropriate type
        if value == 'true' then value = true
        elseif value == 'false' then value = false
        elseif tonumber(value) then value = tonumber(value)
        end
        
        if not serverDefaults[category] then
            serverDefaults[category] = {}
        end
        
        serverDefaults[category][key] = value
        SaveServerDefaults()
        
        print('[CBPS Menu] Server default updated: ' .. category .. '.' .. key .. ' = ' .. tostring(value))
        
        if source ~= 0 then
            TriggerClientEvent('cbps:showNotification', source, '~g~Server default updated')
        end
    else
        TriggerClientEvent('cbps:showNotification', source, '~r~Admin only command')
    end
end, false)

-- Admin command to reset server defaults
RegisterCommand('cbps_reset_defaults', function(source, args, rawCommand)
    if source == 0 or IsPlayerAdmin(source) then
        SaveServerDefaults()
        print('[CBPS Menu] Server defaults reset and saved')
        
        if source ~= 0 then
            TriggerClientEvent('cbps:showNotification', source, '~g~Server defaults reset')
        end
    else
        TriggerClientEvent('cbps:showNotification', source, '~r~Admin only command')
    end
end, false)

-- Helper function to check if player is admin
function IsPlayerAdmin(playerId)
    if not Config.AdminOnly then
        return true
    end
    
    local identifiers = GetPlayerIdentifiers(playerId)
    for _, identifier in pairs(identifiers) do
        for _, admin in pairs(Config.Admins) do
            if identifier == admin then
                return true
            end
        end
    end
    
    return false
end

-- Initialize
Citizen.CreateThread(function()
    Citizen.Wait(1000)
    LoadServerDefaults()
end)

print('[CBPS Menu] Settings management initialized')
