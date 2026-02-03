-- Client Main Script using LemonUI
local lemon = exports.lemonui
local pool = lemon:CreatePool()
local mainMenu = nil
local currentTheme = Config.DefaultTheme

-- Initialize the menu
Citizen.CreateThread(function()
    -- Wait for LemonUI to be ready
    Citizen.Wait(1000)
    
    -- Create the main menu
    CreateMainMenu()
    
    while true do
        Citizen.Wait(0)
        
        -- Process menu pool
        pool:ProcessMenus()
        
        -- Check for menu key press
        if IsControlJustReleased(0, GetKeyMapping(Config.MenuKey)) then
            if pool:AreAnyVisible() then
                pool:CloseAllMenus()
            else
                mainMenu:Visible(true)
            end
        end
    end
end)

function GetKeyMapping(key)
    local keys = {
        ['F1'] = 288,
        ['F2'] = 289,
        ['F3'] = 170,
        ['F5'] = 166,
        ['F6'] = 167,
        ['F7'] = 168,
        ['F10'] = 57,
    }
    return keys[key] or 288
end

function CreateMainMenu()
    mainMenu = lemon:CreateMenu(Config.MenuTitle, '~b~Main Menu')
    ApplyTheme(mainMenu)
    pool:AddMenu(mainMenu)
end

function ApplyTheme(menu)
    local theme = Config.Themes[currentTheme]
    if theme then
        -- LemonUI will handle the theme colors based on banner settings
        menu.Banner:SetColor(theme.banner.r, theme.banner.g, theme.banner.b, theme.banner.a)
    end
end

function ChangeTheme(themeName)
    if Config.Themes[themeName] then
        currentTheme = themeName
        ShowNotification('~g~Theme changed to: ' .. Config.Themes[themeName].name)
        -- Refresh all menus with new theme
        pool:RefreshIndex()
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
