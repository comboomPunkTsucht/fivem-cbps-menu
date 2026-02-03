-- Player Settings Management Client

local playerSettings = {
    -- Theme preferences
    theme = {
        current = nil,
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

local serverDefaults = nil
local settingsLoaded = false

-- Load settings from storage
function LoadPlayerSettings()
    local data = GetResourceKvpString('cbps_player_settings')
    if data then
        local loaded = json.decode(data)
        if loaded then
            -- Merge loaded settings with defaults
            playerSettings = MergeSettings(playerSettings, loaded)
            settingsLoaded = true
            print('[CBPS Menu] Player settings loaded')
        end
    else
        print('[CBPS Menu] No saved settings, using defaults')
    end
    
    -- Request server defaults
    TriggerServerEvent('cbps:requestServerDefaults')
end

-- Save settings to storage
function SavePlayerSettings()
    local data = json.encode(playerSettings, {indent = true})
    SetResourceKvp('cbps_player_settings', data)
    print('[CBPS Menu] Player settings saved')
end

-- Merge settings (deep merge)
function MergeSettings(target, source)
    for key, value in pairs(source) do
        if type(value) == 'table' and type(target[key]) == 'table' then
            target[key] = MergeSettings(target[key], value)
        else
            target[key] = value
        end
    end
    return target
end

-- Apply server defaults
RegisterNetEvent('cbps:receiveServerDefaults')
AddEventHandler('cbps:receiveServerDefaults', function(defaults)
    if defaults then
        serverDefaults = defaults
        
        -- Apply server defaults if no player settings exist
        if not settingsLoaded then
            playerSettings = MergeSettings(playerSettings, defaults)
            print('[CBPS Menu] Server defaults applied')
        end
        
        -- Always update certain server-controlled settings
        if defaults.world and defaults.world.syncWithServer then
            playerSettings.world.syncWithServer = true
        end
    end
end)

-- Get setting value
function GetSetting(category, key)
    if playerSettings[category] and playerSettings[category][key] ~= nil then
        return playerSettings[category][key]
    end
    return nil
end

-- Set setting value
function SetSetting(category, key, value)
    if not playerSettings[category] then
        playerSettings[category] = {}
    end
    
    playerSettings[category][key] = value
    SavePlayerSettings()
    
    -- Notify server of setting change
    TriggerServerEvent('cbps:settingChanged', category, key, value)
end

-- Reset settings to defaults
function ResetSettings(category)
    if category then
        -- Reset specific category
        if serverDefaults and serverDefaults[category] then
            playerSettings[category] = serverDefaults[category]
        else
            playerSettings[category] = {}
        end
    else
        -- Reset all settings
        if serverDefaults then
            playerSettings = MergeSettings({}, serverDefaults)
        end
    end
    
    SavePlayerSettings()
    ShowNotification('~g~Settings reset!')
end

-- Export all settings
function ExportSettings()
    local data = json.encode(playerSettings, {indent = true})
    return data
end

-- Import settings
function ImportSettings(data)
    local imported = json.decode(data)
    if imported then
        playerSettings = MergeSettings(playerSettings, imported)
        SavePlayerSettings()
        ShowNotification('~g~Settings imported!')
        return true
    end
    ShowNotification('~r~Failed to import settings')
    return false
end

-- Auto-save settings periodically
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(60000) -- Every minute
        SavePlayerSettings()
    end
end)

-- Settings menu commands
RegisterCommand('cbps_settings_export', function()
    local data = ExportSettings()
    print('=== CBPS MENU SETTINGS EXPORT ===')
    print(data)
    print('=== END EXPORT ===')
    ShowNotification('~g~Settings exported to console (F8)')
end, false)

RegisterCommand('cbps_settings_reset', function()
    ResetSettings()
    ShowNotification('~g~All settings reset to defaults')
end, false)

RegisterCommand('cbps_settings_save', function()
    SavePlayerSettings()
    ShowNotification('~g~Settings saved manually')
end, false)

-- Initialize settings on resource start
Citizen.CreateThread(function()
    Citizen.Wait(2000)
    LoadPlayerSettings()
end)

-- Save settings on resource stop
AddEventHandler('onResourceStop', function(resourceName)
    if GetCurrentResourceName() == resourceName then
        SavePlayerSettings()
    end
end)

-- Save settings on player disconnect
AddEventHandler('onClientResourceStop', function(resourceName)
    if GetCurrentResourceName() == resourceName then
        SavePlayerSettings()
    end
end)

-- Exports
exports('GetSetting', GetSetting)
exports('SetSetting', SetSetting)
exports('GetAllSettings', function() return playerSettings end)
exports('SaveSettings', SavePlayerSettings)
exports('LoadSettings', LoadPlayerSettings)
exports('ResetSettings', ResetSettings)
exports('ExportSettings', ExportSettings)
exports('ImportSettings', ImportSettings)
