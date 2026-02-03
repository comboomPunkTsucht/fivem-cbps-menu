-- Client Main Script using LemonUI

-- IMPORTANT: Define ShowNotification first, before any code that might fail
-- This ensures all client scripts can use it even if lemonui fails to load
function ShowNotification(msg)
    SetNotificationTextEntry('STRING')
    AddTextComponentString(msg)
    DrawNotification(false, true)
end

-- Export ShowNotification immediately
exports('ShowNotification', ShowNotification)

-- Try to initialize LemonUI with error handling
local lemon = nil
local pool = nil
local mainMenu = nil
local currentTheme = Config.DefaultTheme
local customThemes = {} -- Store player's custom themes
local lemonuiAvailable = false

-- Safely try to get lemonui exports
Citizen.CreateThread(function()
    Citizen.Wait(500) -- Give lemonui time to load
    
    local success, result = pcall(function()
        lemon = exports.lemonui
        pool = lemon:CreatePool()
        return true
    end)
    
    if success and pool then
        lemonuiAvailable = true
        print('[CBPS Menu] LemonUI loaded successfully')
    else
        lemonuiAvailable = false
        print('[CBPS Menu] WARNING: LemonUI not available - menu features disabled')
        print('[CBPS Menu] Please ensure lemonui resource is installed and started')
        ShowNotification('~r~CBPS Menu: LemonUI not available')
    end
end)

-- Register keybinds using FiveM's native keybinding system
RegisterCommand('cbps_menu', function()
    if not lemonuiAvailable or not pool then
        ShowNotification('~r~Menu not available - LemonUI not loaded')
        return
    end
    if pool:AreAnyVisible() then
        pool:CloseAllMenus()
    else
        if mainMenu then
            mainMenu:Visible(true)
        end
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
    -- Wait for LemonUI initialization to complete
    Citizen.Wait(1500)
    
    -- Register key mappings (these work even without lemonui)
    RegisterKeyMapping('cbps_menu', Config.Keybinds.OpenMenu.description, 'keyboard', Config.Keybinds.OpenMenu.key)
    RegisterKeyMapping('cbps_voice_range', Config.Keybinds.VoiceRange.description, 'keyboard', Config.Keybinds.VoiceRange.key)
    
    if Config.Keybinds.Noclip.key then
        RegisterKeyMapping('cbps_noclip', Config.Keybinds.Noclip.description, 'keyboard', Config.Keybinds.Noclip.key)
    end
    
    -- Only create menu if lemonui is available
    if lemonuiAvailable then
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
    else
        -- Even without lemonui, keep the thread alive for potential future recovery
        print('[CBPS Menu] Menu loop not started - LemonUI unavailable')
    end
end)

-- Handle controller input for menu
function HandleControllerInput()
    if not lemonuiAvailable or not pool then return end
    if not pool:AreAnyVisible() then
        -- Check for controller menu open button
        if IsControlJustReleased(0, GetControllerButton(Config.Controller.OpenMenu)) then
            if mainMenu then
                mainMenu:Visible(true)
            end
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
    if not lemonuiAvailable or not lemon or not pool then return end
    mainMenu = lemon:CreateMenu(Config.MenuTitle, '~b~Main Menu')
    ApplyTheme(mainMenu)
    pool:AddMenu(mainMenu)
end

function ApplyTheme(menu)
    if not menu then return end
    local theme = Config.Themes[currentTheme] or customThemes[currentTheme]
    if theme and menu.Banner then
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
        if pool then
            pool:RefreshIndex()
        end
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

-- Export functions (ShowNotification already exported at top of file)
exports('GetMenuPool', function() return pool end)
exports('GetMainMenu', function() return mainMenu end)
exports('ApplyTheme', ApplyTheme)
exports('ChangeTheme', ChangeTheme)
exports('GetCurrentTheme', function() return currentTheme end)
exports('AddCustomTheme', AddCustomTheme)
exports('GetCustomThemes', function() return customThemes end)
exports('IsLemonUIAvailable', function() return lemonuiAvailable end)
