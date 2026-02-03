-- Client Main Script using LemonUI
local lemon = exports.lemonui
local pool = lemon:CreatePool()
local mainMenu = nil
local currentTheme = Config.DefaultTheme
local customThemes = {} -- Store player's custom themes

-- Register keybinds using FiveM's native keybinding system
RegisterCommand('cbps_menu', function()
    if pool:AreAnyVisible() then
        pool:CloseAllMenus()
    else
        mainMenu:Visible(true)
    end
end, false)

RegisterCommand('cbps_voice_range', function()
    TriggerEvent('cbps:cycleVoiceRange')
end, false)

RegisterCommand('cbps_noclip', function()
    if Config.Keybinds.Noclip.key then
        ToggleNoclip() -- Function from player.lua
    end
end, false)

-- Initialize the menu
Citizen.CreateThread(function()
    -- Wait for LemonUI to be ready
    Citizen.Wait(1000)
    
    -- Register key mappings
    RegisterKeyMapping('cbps_menu', Config.Keybinds.OpenMenu.description, 'keyboard', Config.Keybinds.OpenMenu.key)
    RegisterKeyMapping('cbps_voice_range', Config.Keybinds.VoiceRange.description, 'keyboard', Config.Keybinds.VoiceRange.key)
    
    if Config.Keybinds.Noclip.key then
        RegisterKeyMapping('cbps_noclip', Config.Keybinds.Noclip.description, 'keyboard', Config.Keybinds.Noclip.key)
    end
    
    -- Create the main menu
    CreateMainMenu()
    
    -- Load custom themes from storage
    LoadCustomThemes()
    
    while true do
        Citizen.Wait(0)
        
        -- Process menu pool
        pool:ProcessMenus()
        
        -- Controller support
        if Config.Controller.Enabled then
            HandleControllerInput()
        end
    end
end)

-- Handle controller input for menu
function HandleControllerInput()
    if not pool:AreAnyVisible() then
        -- Check for controller menu open button
        if IsControlJustReleased(0, GetControllerButton(Config.Controller.OpenMenu)) then
            mainMenu:Visible(true)
        end
    end
end

function GetControllerButton(button)
    local buttons = {
        ['DPAD_UP'] = 27,
        ['DPAD_DOWN'] = 28,
        ['DPAD_LEFT'] = 29,
        ['DPAD_RIGHT'] = 30,
        ['A'] = 191, -- Xbox A / PS X
        ['B'] = 194, -- Xbox B / PS Circle
        ['X'] = 192, -- Xbox X / PS Square
        ['Y'] = 193, -- Xbox Y / PS Triangle
        ['BACK'] = 199, -- Xbox Back / PS Select
        ['START'] = 200, -- Xbox Start / PS Start
        ['LB'] = 14, -- Left bumper
        ['RB'] = 15, -- Right bumper
        ['LT'] = 11, -- Left trigger
        ['RT'] = 12, -- Right trigger
    }
    return buttons[button] or 199
end

function GetKeyMapping(key)
    local keys = {
        ['F1'] = 288,
        ['F2'] = 289,
        ['F3'] = 170,
        ['F5'] = 166,
        ['F6'] = 167,
        ['F7'] = 168,
        ['F10'] = 57,
        ['LMENU'] = 19, -- Left ALT
        ['RMENU'] = 19, -- Right ALT (same as left)
    }
    return keys[key] or 288
end

function CreateMainMenu()
    mainMenu = lemon:CreateMenu(Config.MenuTitle, '~b~Main Menu')
    ApplyTheme(mainMenu)
    pool:AddMenu(mainMenu)
end

function ApplyTheme(menu)
    local theme = Config.Themes[currentTheme] or customThemes[currentTheme]
    if theme then
        -- LemonUI will handle the theme colors based on banner settings
        menu.Banner:SetColor(theme.banner.r, theme.banner.g, theme.banner.b, theme.banner.a)
    end
end

function ChangeTheme(themeName)
    local theme = Config.Themes[themeName] or customThemes[themeName]
    if theme then
        currentTheme = themeName
        ShowNotification('~g~Theme changed to: ' .. theme.name)
        -- Refresh all menus with new theme
        pool:RefreshIndex()
        -- Save theme preference
        SaveThemePreference(themeName)
    end
end

function AddCustomTheme(themeName, themeData)
    if Config.AllowCustomThemes then
        customThemes[themeName] = themeData
        SaveCustomThemes()
        ShowNotification('~g~Custom theme "' .. themeName .. '" created!')
        return true
    end
    return false
end

-- Save/Load Custom Themes
function SaveCustomThemes()
    local data = json.encode(customThemes)
    SetResourceKvp('cbps_custom_themes', data)
end

function LoadCustomThemes()
    local data = GetResourceKvpString('cbps_custom_themes')
    if data then
        customThemes = json.decode(data) or {}
    end
end

function SaveThemePreference(themeName)
    SetResourceKvp('cbps_current_theme', themeName)
end

function LoadThemePreference()
    local savedTheme = GetResourceKvpString('cbps_current_theme')
    if savedTheme then
        currentTheme = savedTheme
    end
end

-- Notifications
function ShowNotification(msg)
    SetNotificationTextEntry('STRING')
    AddTextComponentString(msg)
    DrawNotification(false, true)
end

-- Export functions
exports('ShowNotification', ShowNotification)
exports('GetMenuPool', function() return pool end)
exports('GetMainMenu', function() return mainMenu end)
exports('ApplyTheme', ApplyTheme)
exports('ChangeTheme', ChangeTheme)
exports('GetCurrentTheme', function() return currentTheme end)
exports('AddCustomTheme', AddCustomTheme)
exports('GetCustomThemes', function() return customThemes end)
