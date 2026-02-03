-- Menu Management using LemonUI
local lemon = exports.lemonui
local pool = nil
local mainMenu = nil

-- Submenus
local playerMenu = nil
local vehicleMenu = nil
local vehicleSpawnerMenu = nil
local weaponsMenu = nil
local voiceMenu = nil
local teamMenu = nil
local raceMenu = nil
local worldMenu = nil
local settingsMenu = nil

Citizen.CreateThread(function()
    Citizen.Wait(2000)
    
    pool = exports['cbps-menu']:GetMenuPool()
    mainMenu = exports['cbps-menu']:GetMainMenu()
    
    if mainMenu then
        BuildMenus()
    end
end)

function BuildMenus()
    -- Player Options Menu
    playerMenu = lemon:CreateSubMenu(mainMenu, 'Player Options', '~b~Manage your player')
    pool:AddSubMenu(playerMenu)
    exports['cbps-menu']:ApplyTheme(playerMenu)
    
    local healItem = lemon:CreateItem('Heal Player', 'Restore health to maximum')
    playerMenu:AddItem(healItem)
    
    local armorItem = lemon:CreateItem('Give Armor', 'Give full armor')
    playerMenu:AddItem(armorItem)
    
    local godmodeItem = lemon:CreateCheckboxItem('God Mode', 'Toggle invincibility', false)
    playerMenu:AddItem(godmodeItem)
    
    local invisibleItem = lemon:CreateCheckboxItem('Invisible', 'Toggle invisibility', false)
    playerMenu:AddItem(invisibleItem)
    
    local noclipItem = lemon:CreateCheckboxItem('Noclip', 'Toggle noclip mode', false)
    playerMenu:AddItem(noclipItem)
    
    local superjumpItem = lemon:CreateCheckboxItem('Super Jump', 'Toggle super jump', false)
    playerMenu:AddItem(superjumpItem)
    
    local fastrunItem = lemon:CreateCheckboxItem('Fast Run', 'Toggle fast run', false)
    playerMenu:AddItem(fastrunItem)
    
    local teleportItem = lemon:CreateItem('Teleport to Waypoint', 'Teleport to your waypoint')
    playerMenu:AddItem(teleportItem)
    
    local clearwantedItem = lemon:CreateItem('Clear Wanted Level', 'Remove wanted level')
    playerMenu:AddItem(clearwantedItem)
    
    local suicideItem = lemon:CreateItem('Suicide', '~r~Kill yourself')
    playerMenu:AddItem(suicideItem)
    
    -- Vehicle Options Menu
    vehicleMenu = lemon:CreateSubMenu(mainMenu, 'Vehicle Options', '~b~Manage vehicles')
    pool:AddSubMenu(vehicleMenu)
    exports['cbps-menu']:ApplyTheme(vehicleMenu)
    
    local spawnerItem = lemon:CreateItem('Vehicle Spawner', 'Spawn a vehicle')
    vehicleMenu:AddItem(spawnerItem)
    
    local repairItem = lemon:CreateItem('Repair Vehicle', 'Fix current vehicle')
    vehicleMenu:AddItem(repairItem)
    
    local cleanItem = lemon:CreateItem('Clean Vehicle', 'Clean current vehicle')
    vehicleMenu:AddItem(cleanItem)
    
    local flipItem = lemon:CreateItem('Flip Vehicle', 'Flip vehicle right-side up')
    vehicleMenu:AddItem(flipItem)
    
    local boostItem = lemon:CreateItem('Boost Vehicle', 'Give vehicle a speed boost')
    vehicleMenu:AddItem(boostItem)
    
    local maxupgradeItem = lemon:CreateItem('Max Upgrade', 'Fully upgrade current vehicle')
    vehicleMenu:AddItem(maxupgradeItem)
    
    local vehInvincibleItem = lemon:CreateCheckboxItem('Vehicle Invincible', 'Toggle vehicle invincibility', false)
    vehicleMenu:AddItem(vehInvincibleItem)
    
    local rainbowItem = lemon:CreateCheckboxItem('Rainbow Paint', 'Toggle rainbow paint', false)
    vehicleMenu:AddItem(rainbowItem)
    
    local deleteItem = lemon:CreateItem('Delete Vehicle', '~r~Delete current vehicle')
    vehicleMenu:AddItem(deleteItem)
    
    -- Vehicle Spawner Submenu
    vehicleSpawnerMenu = lemon:CreateSubMenu(vehicleMenu, 'Vehicle Spawner', '~b~Choose a vehicle category')
    pool:AddSubMenu(vehicleSpawnerMenu)
    exports['cbps-menu']:ApplyTheme(vehicleSpawnerMenu)
    
    -- Add vehicle categories
    for _, category in pairs(Config.VehicleCategories) do
        local categoryMenu = lemon:CreateSubMenu(vehicleSpawnerMenu, category.name, '~b~' .. category.name .. ' Vehicles')
        pool:AddSubMenu(categoryMenu)
        exports['cbps-menu']:ApplyTheme(categoryMenu)
        
        for _, vehicle in pairs(category.vehicles) do
            local vehItem = lemon:CreateItem(vehicle, 'Spawn ' .. vehicle)
            categoryMenu:AddItem(vehItem)
            categoryMenu.OnItemSelect = function(sender, item, index)
                TriggerEvent('cbps:spawnVehicle', category.vehicles[index])
            end
        end
    end
    
    -- Weapons Menu
    weaponsMenu = lemon:CreateSubMenu(mainMenu, 'Weapon Options', '~b~Manage weapons')
    pool:AddSubMenu(weaponsMenu)
    exports['cbps-menu']:ApplyTheme(weaponsMenu)
    
    local giveAllWeaponsItem = lemon:CreateItem('Give All Weapons', 'Give all weapons')
    weaponsMenu:AddItem(giveAllWeaponsItem)
    
    local removeAllWeaponsItem = lemon:CreateItem('Remove All Weapons', '~r~Remove all weapons')
    weaponsMenu:AddItem(removeAllWeaponsItem)
    
    local infiniteAmmoItem = lemon:CreateCheckboxItem('Infinite Ammo', 'Toggle infinite ammo', false)
    weaponsMenu:AddItem(infiniteAmmoItem)
    
    local noReloadItem = lemon:CreateCheckboxItem('No Reload', 'Toggle no reload', false)
    weaponsMenu:AddItem(noReloadItem)
    
    -- Add weapon categories
    for _, category in pairs(Config.WeaponCategories) do
        local categoryMenu = lemon:CreateSubMenu(weaponsMenu, category.name, '~b~' .. category.name)
        pool:AddSubMenu(categoryMenu)
        exports['cbps-menu']:ApplyTheme(categoryMenu)
        
        for _, weapon in pairs(category.weapons) do
            local weapItem = lemon:CreateItem(weapon, 'Give ' .. weapon)
            categoryMenu:AddItem(weapItem)
            categoryMenu.OnItemSelect = function(sender, item, index)
                TriggerEvent('cbps:giveWeapon', category.weapons[index])
            end
        end
    end
    
    -- Voice & Radio Menu
    if Config.Voice.Enabled or Config.Radio.Enabled then
        voiceMenu = lemon:CreateSubMenu(mainMenu, 'Voice & Radio', '~b~Voice and radio controls')
        pool:AddSubMenu(voiceMenu)
        exports['cbps-menu']:ApplyTheme(voiceMenu)
        
        if Config.Voice.Enabled then
            local voiceRangeList = lemon:CreateListItem('Voice Range', Config.Voice.Ranges, 2)
            voiceMenu:AddItem(voiceRangeList)
            
            local toggleVoiceItem = lemon:CreateItem('Toggle Voice', 'Mute/unmute voice')
            voiceMenu:AddItem(toggleVoiceItem)
        end
        
        if Config.Radio.Enabled then
            local radioFreqItem = lemon:CreateItem('Set Radio Frequency', 'Enter radio frequency')
            voiceMenu:AddItem(radioFreqItem)
            
            local radioOffItem = lemon:CreateItem('Turn Off Radio', 'Disconnect from radio')
            voiceMenu:AddItem(radioOffItem)
        end
    end
    
    -- Team Management Menu
    if Config.Team.Enabled then
        teamMenu = lemon:CreateSubMenu(mainMenu, 'Team Management', '~b~Manage your team')
        pool:AddSubMenu(teamMenu)
        exports['cbps-menu']:ApplyTheme(teamMenu)
        
        local createTeamItem = lemon:CreateItem('Create Team', 'Create a new team')
        teamMenu:AddItem(createTeamItem)
        
        local leaveTeamItem = lemon:CreateItem('Leave Team', 'Leave current team')
        teamMenu:AddItem(leaveTeamItem)
        
        local invitePlayerItem = lemon:CreateItem('Invite Player', 'Invite nearby player')
        teamMenu:AddItem(invitePlayerItem)
        
        local kickPlayerItem = lemon:CreateItem('Kick Player', 'Kick team member')
        teamMenu:AddItem(kickPlayerItem)
        
        local teamChatItem = lemon:CreateItem('Team Chat', 'Send team message')
        teamMenu:AddItem(teamChatItem)
    end
    
    -- Race Functions Menu
    if Config.Race.Enabled then
        raceMenu = lemon:CreateSubMenu(mainMenu, 'Race Functions', '~b~Race management')
        pool:AddSubMenu(raceMenu)
        exports['cbps-menu']:ApplyTheme(raceMenu)
        
        local createRaceItem = lemon:CreateItem('Create Race', 'Start creating a race')
        raceMenu:AddItem(createRaceItem)
        
        local joinRaceItem = lemon:CreateItem('Join Race', 'Join available race')
        raceMenu:AddItem(joinRaceItem)
        
        local leaveRaceItem = lemon:CreateItem('Leave Race', 'Leave current race')
        raceMenu:AddItem(leaveRaceItem)
        
        local startRaceItem = lemon:CreateItem('Start Race', 'Start the race countdown')
        raceMenu:AddItem(startRaceItem)
        
        local addCheckpointItem = lemon:CreateItem('Add Checkpoint', 'Add checkpoint at current location')
        raceMenu:AddItem(addCheckpointItem)
        
        local clearCheckpointsItem = lemon:CreateItem('Clear Checkpoints', 'Remove all checkpoints')
        raceMenu:AddItem(clearCheckpointsItem)
    end
    
    -- World Options Menu
    worldMenu = lemon:CreateSubMenu(mainMenu, 'World Options', '~b~World settings')
    pool:AddSubMenu(worldMenu)
    exports['cbps-menu']:ApplyTheme(worldMenu)
    
    local weatherList = lemon:CreateListItem('Weather', {
        'EXTRASUNNY', 'CLEAR', 'CLOUDS', 'OVERCAST', 'RAIN', 'THUNDER', 'CLEARING', 'NEUTRAL', 'SNOW', 'BLIZZARD', 'SNOWLIGHT', 'XMAS'
    }, 1)
    worldMenu:AddItem(weatherList)
    
    local timeItem = lemon:CreateItem('Set Time', 'Change time of day')
    worldMenu:AddItem(timeItem)
    
    -- Settings Menu
    if Config.AllowThemeChange then
        settingsMenu = lemon:CreateSubMenu(mainMenu, 'Settings', '~b~Menu settings')
        pool:AddSubMenu(settingsMenu)
        exports['cbps-menu']:ApplyTheme(settingsMenu)
        
        local themeNames = {}
        for themeName, themeData in pairs(Config.Themes) do
            table.insert(themeNames, themeData.name)
        end
        
        local themeList = lemon:CreateListItem('Menu Theme', themeNames, 1)
        settingsMenu:AddItem(themeList)
    end
    
    -- Event handlers
    SetupEventHandlers()
end

function SetupEventHandlers()
    -- Player menu events
    if playerMenu then
        playerMenu.OnItemSelect = function(sender, item, index)
            if index == 1 then HealPlayer()
            elseif index == 2 then GiveArmor()
            elseif index == 8 then TeleportToWaypoint()
            elseif index == 9 then ClearWantedLevel()
            elseif index == 10 then SuicidePlayer()
            end
        end
        
        playerMenu.OnCheckboxChange = function(sender, item, checked, index)
            if index == 3 then ToggleGodMode()
            elseif index == 4 then ToggleInvisible()
            elseif index == 5 then ToggleNoclip()
            elseif index == 6 then ToggleSuperJump()
            elseif index == 7 then ToggleFastRun()
            end
        end
    end
    
    -- Vehicle menu events
    if vehicleMenu then
        vehicleMenu.OnItemSelect = function(sender, item, index)
            if index == 1 then -- Spawner submenu, do nothing
            elseif index == 2 then RepairVehicle()
            elseif index == 3 then CleanVehicle()
            elseif index == 4 then FlipVehicle()
            elseif index == 5 then BoostVehicle()
            elseif index == 6 then MaxUpgradeVehicle()
            elseif index == 9 then DeleteCurrentVehicle()
            end
        end
        
        vehicleMenu.OnCheckboxChange = function(sender, item, checked, index)
            if index == 7 then ToggleVehicleInvincible()
            elseif index == 8 then ToggleRainbowVehicle()
            end
        end
    end
    
    -- Weapons menu events
    if weaponsMenu then
        weaponsMenu.OnItemSelect = function(sender, item, index)
            if index == 1 then GiveAllWeapons()
            elseif index == 2 then RemoveAllWeapons()
            end
        end
        
        weaponsMenu.OnCheckboxChange = function(sender, item, checked, index)
            if index == 3 then ToggleInfiniteAmmo()
            elseif index == 4 then ToggleNoReload()
            end
        end
    end
    
    -- Voice menu events
    if voiceMenu and Config.Voice.Enabled then
        voiceMenu.OnListChange = function(sender, item, index)
            if item.Text == 'Voice Range' then
                SetVoiceRange(Config.Voice.Ranges[index])
            end
        end
        
        voiceMenu.OnItemSelect = function(sender, item, index)
            local offset = Config.Voice.Enabled and 2 or 0
            if Config.Radio.Enabled then
                if index == offset + 1 then
                    DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP8", "", "", "", "", "", 64)
                    while UpdateOnscreenKeyboard() == 0 do
                        Citizen.Wait(0)
                    end
                    if GetOnscreenKeyboardResult() then
                        local frequency = tonumber(GetOnscreenKeyboardResult())
                        if frequency then
                            SetRadioFrequency(frequency)
                        end
                    end
                elseif index == offset + 2 then
                    TurnOffRadio()
                end
            end
        end
    end
    
    -- Team menu events
    if teamMenu and Config.Team.Enabled then
        teamMenu.OnItemSelect = function(sender, item, index)
            if index == 1 then TriggerServerEvent('cbps:createTeam')
            elseif index == 2 then TriggerServerEvent('cbps:leaveTeam')
            elseif index == 3 then
                -- Get nearby players and invite
                TriggerServerEvent('cbps:inviteNearbyPlayer')
            elseif index == 4 then
                -- Show team members to kick
                TriggerServerEvent('cbps:requestTeamMembers')
            elseif index == 5 then
                -- Team chat
                DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP8", "", "", "", "", "", 128)
                while UpdateOnscreenKeyboard() == 0 do
                    Citizen.Wait(0)
                end
                if GetOnscreenKeyboardResult() then
                    local message = GetOnscreenKeyboardResult()
                    TriggerServerEvent('cbps:teamChat', message)
                end
            end
        end
    end
    
    -- Race menu events
    if raceMenu and Config.Race.Enabled then
        raceMenu.OnItemSelect = function(sender, item, index)
            if index == 1 then TriggerServerEvent('cbps:createRace')
            elseif index == 2 then TriggerServerEvent('cbps:joinRace')
            elseif index == 3 then TriggerServerEvent('cbps:leaveRace')
            elseif index == 4 then TriggerServerEvent('cbps:startRace')
            elseif index == 5 then TriggerEvent('cbps:addCheckpoint')
            elseif index == 6 then TriggerEvent('cbps:clearCheckpoints')
            end
        end
    end
    
    -- World menu events
    if worldMenu then
        worldMenu.OnListChange = function(sender, item, index)
            if item.Text == 'Weather' then
                local weathers = {'EXTRASUNNY', 'CLEAR', 'CLOUDS', 'OVERCAST', 'RAIN', 'THUNDER', 'CLEARING', 'NEUTRAL', 'SNOW', 'BLIZZARD', 'SNOWLIGHT', 'XMAS'}
                TriggerServerEvent('cbps:changeWeather', weathers[index])
            end
        end
        
        worldMenu.OnItemSelect = function(sender, item, index)
            if index == 2 then
                DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP8", "", "Enter hour (0-23)", "", "", "", 2)
                while UpdateOnscreenKeyboard() == 0 do
                    Citizen.Wait(0)
                end
                if GetOnscreenKeyboardResult() then
                    local hour = tonumber(GetOnscreenKeyboardResult())
                    if hour and hour >= 0 and hour <= 23 then
                        TriggerServerEvent('cbps:changeTime', hour, 0)
                    end
                end
            end
        end
    end
    
    -- Settings menu events
    if settingsMenu and Config.AllowThemeChange then
        settingsMenu.OnListChange = function(sender, item, index)
            if item.Text == 'Menu Theme' then
                local themeKeys = {}
                for themeName, _ in pairs(Config.Themes) do
                    table.insert(themeKeys, themeName)
                end
                exports['cbps-menu']:ChangeTheme(themeKeys[index])
                -- Reapply theme to all menus
                for _, menu in pairs(pool.Menus) do
                    exports['cbps-menu']:ApplyTheme(menu)
                end
            end
        end
    end
end
