-- Server Race Management

local races = {}
local playerRaces = {}
local raceResults = {}
local nextRaceId = 1
local savedRaceTemplates = {} -- Persistent race templates

-- Load saved races on server start
Citizen.CreateThread(function()
    if Config.Race.SaveRaces then
        LoadSavedRaces()
    end
end)

-- Save/Load race templates
function SaveRacesToFile()
    if not Config.Race.SaveRaces then return end
    
    local data = json.encode(savedRaceTemplates, {indent = true})
    SaveResourceFile(GetCurrentResourceName(), Config.Race.RaceSaveFile, data, -1)
    print('[CBPS Menu] Saved ' .. #savedRaceTemplates .. ' race templates')
end

function LoadSavedRaces()
    local data = LoadResourceFile(GetCurrentResourceName(), Config.Race.RaceSaveFile)
    if data then
        savedRaceTemplates = json.decode(data) or {}
        print('[CBPS Menu] Loaded ' .. #savedRaceTemplates .. ' race templates')
    else
        savedRaceTemplates = {}
        print('[CBPS Menu] No saved races found, starting fresh')
    end
end

-- Save race template
RegisterNetEvent('cbps:saveRaceTemplate')
AddEventHandler('cbps:saveRaceTemplate', function(raceName)
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~No active race to save')
        return
    end
    
    local race = races[raceId]
    if not race or race.creator ~= playerId then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~You can only save races you created')
        return
    end
    
    if #race.checkpoints == 0 then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~Cannot save race with no checkpoints')
        return
    end
    
    -- Create template
    local template = {
        name = raceName,
        checkpoints = race.checkpoints,
        createdBy = GetPlayerName(playerId),
        createdAt = os.time()
    }
    
    table.insert(savedRaceTemplates, template)
    SaveRacesToFile()
    
    TriggerClientEvent('cbps:showNotification', playerId, '~g~Race template "' .. raceName .. '" saved!')
    print('[CBPS Menu] Race template "' .. raceName .. '" saved by ' .. GetPlayerName(playerId))
end)

-- Load race template
RegisterNetEvent('cbps:loadRaceTemplate')
AddEventHandler('cbps:loadRaceTemplate', function(templateIndex)
    local playerId = source
    
    if not savedRaceTemplates[templateIndex] then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~Race template not found')
        return
    end
    
    local template = savedRaceTemplates[templateIndex]
    
    -- Create new race from template
    local raceId = nextRaceId
    nextRaceId = nextRaceId + 1
    
    races[raceId] = {
        id = raceId,
        creator = playerId,
        checkpoints = template.checkpoints,
        participants = {playerId},
        started = false,
        finished = {},
        createdAt = os.time(),
        templateName = template.name
    }
    
    playerRaces[playerId] = raceId
    
    TriggerClientEvent('cbps:raceCreated', playerId, raceId)
    TriggerClientEvent('cbps:raceTemplateLoaded', playerId, template.checkpoints)
    TriggerClientEvent('cbps:showNotification', playerId, '~g~Loaded race template: ' .. template.name)
    print('[CBPS Menu] Race template "' .. template.name .. '" loaded by ' .. GetPlayerName(playerId))
end)

-- Get saved race templates
RegisterNetEvent('cbps:getSavedRaceTemplates')
AddEventHandler('cbps:getSavedRaceTemplates', function()
    local playerId = source
    TriggerClientEvent('cbps:receiveSavedRaceTemplates', playerId, savedRaceTemplates)
end)

-- Delete race template
RegisterNetEvent('cbps:deleteRaceTemplate')
AddEventHandler('cbps:deleteRaceTemplate', function(templateIndex)
    local playerId = source
    
    if not savedRaceTemplates[templateIndex] then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~Race template not found')
        return
    end
    
    local templateName = savedRaceTemplates[templateIndex].name
    table.remove(savedRaceTemplates, templateIndex)
    SaveRacesToFile()
    
    TriggerClientEvent('cbps:showNotification', playerId, '~g~Deleted race template: ' .. templateName)
    print('[CBPS Menu] Race template "' .. templateName .. '" deleted by ' .. GetPlayerName(playerId))
end)

-- Create race
RegisterNetEvent('cbps:createRace')
AddEventHandler('cbps:createRace', function()
    local playerId = source
    local playerName = GetPlayerName(playerId)
    
    -- Create new race
    local raceId = nextRaceId
    nextRaceId = nextRaceId + 1
    
    races[raceId] = {
        id = raceId,
        creator = playerId,
        checkpoints = {},
        participants = {playerId},
        started = false,
        finished = {},
        createdAt = os.time()
    }
    
    playerRaces[playerId] = raceId
    
    TriggerClientEvent('cbps:raceCreated', playerId, raceId)
    print('[CBPS Menu] Race created by ' .. playerName .. ' (ID: ' .. raceId .. ')')
end)

-- Add checkpoint
RegisterNetEvent('cbps:addRaceCheckpoint')
AddEventHandler('cbps:addRaceCheckpoint', function(coords)
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        return
    end
    
    local race = races[raceId]
    if not race or race.creator ~= playerId then
        return
    end
    
    table.insert(race.checkpoints, coords)
end)

-- Clear checkpoints
RegisterNetEvent('cbps:clearRaceCheckpoints')
AddEventHandler('cbps:clearRaceCheckpoints', function()
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        return
    end
    
    local race = races[raceId]
    if not race or race.creator ~= playerId then
        return
    end
    
    race.checkpoints = {}
end)

-- Join race
RegisterNetEvent('cbps:joinRace')
AddEventHandler('cbps:joinRace', function()
    local playerId = source
    
    -- Find available race (not started)
    local availableRace = nil
    for _, race in pairs(races) do
        if not race.started and #race.checkpoints > 0 then
            availableRace = race
            break
        end
    end
    
    if not availableRace then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~No available races')
        return
    end
    
    -- Add player to race
    table.insert(availableRace.participants, playerId)
    playerRaces[playerId] = availableRace.id
    
    TriggerClientEvent('cbps:joinedRace', playerId, availableRace.id, availableRace.checkpoints)
end)

-- Leave race
RegisterNetEvent('cbps:leaveRace')
AddEventHandler('cbps:leaveRace', function()
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        return
    end
    
    local race = races[raceId]
    if not race then
        return
    end
    
    -- Remove player from race
    for i, participantId in ipairs(race.participants) do
        if participantId == playerId then
            table.remove(race.participants, i)
            break
        end
    end
    
    playerRaces[playerId] = nil
    
    TriggerClientEvent('cbps:leftRace', playerId)
    
    -- Delete race if empty
    if #race.participants == 0 then
        races[raceId] = nil
        print('[CBPS Menu] Race deleted (ID: ' .. raceId .. ')')
    end
end)

-- Start race
RegisterNetEvent('cbps:startRace')
AddEventHandler('cbps:startRace', function()
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        return
    end
    
    local race = races[raceId]
    if not race or race.creator ~= playerId then
        return
    end
    
    if #race.checkpoints == 0 then
        TriggerClientEvent('cbps:showNotification', playerId, '~r~No checkpoints set')
        return
    end
    
    race.started = true
    race.startTime = os.time()
    
    -- Notify all participants
    for _, participantId in ipairs(race.participants) do
        TriggerClientEvent('cbps:raceStarted', participantId, Config.Race.CountdownTime)
    end
    
    print('[CBPS Menu] Race started (ID: ' .. raceId .. ')')
end)

-- Reached checkpoint
RegisterNetEvent('cbps:reachedCheckpoint')
AddEventHandler('cbps:reachedCheckpoint', function(checkpointNum)
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        return
    end
    
    TriggerClientEvent('cbps:checkpointReached', playerId, checkpointNum)
end)

-- Finish race
RegisterNetEvent('cbps:finishRace')
AddEventHandler('cbps:finishRace', function(time)
    local playerId = source
    local raceId = playerRaces[playerId]
    
    if not raceId then
        return
    end
    
    local race = races[raceId]
    if not race then
        return
    end
    
    -- Record finish time
    table.insert(race.finished, {
        playerId = playerId,
        playerName = GetPlayerName(playerId),
        time = time
    })
    
    local position = #race.finished
    
    TriggerClientEvent('cbps:raceFinished', playerId, position, time)
    
    -- Notify all participants
    for _, participantId in ipairs(race.participants) do
        if participantId ~= playerId then
            TriggerClientEvent('cbps:showNotification', participantId, 
                '~b~' .. GetPlayerName(playerId) .. ' finished in position ' .. position)
        end
    end
    
    playerRaces[playerId] = nil
end)

-- Player dropped - clean up race
AddEventHandler('cbps:playerDropped', function(playerId)
    local raceId = playerRaces[playerId]
    if raceId then
        local race = races[raceId]
        if race then
            -- Remove player from race
            for i, participantId in ipairs(race.participants) do
                if participantId == playerId then
                    table.remove(race.participants, i)
                    break
                end
            end
            
            -- Delete race if empty or creator left
            if #race.participants == 0 or race.creator == playerId then
                for _, participantId in ipairs(race.participants) do
                    TriggerClientEvent('cbps:leftRace', participantId)
                    playerRaces[participantId] = nil
                end
                races[raceId] = nil
            end
        end
        
        playerRaces[playerId] = nil
    end
end)

-- Change weather (sync to all players)
RegisterNetEvent('cbps:changeWeather')
AddEventHandler('cbps:changeWeather', function(weather)
    local playerId = source
    
    -- Broadcast to all players
    TriggerClientEvent('cbps:weatherChanged', -1, weather)
    
    print('[CBPS Menu] Weather changed to ' .. weather .. ' by ' .. GetPlayerName(playerId))
end)

-- Change time (sync to all players)
RegisterNetEvent('cbps:changeTime')
AddEventHandler('cbps:changeTime', function(hour, minute)
    local playerId = source
    
    -- Broadcast to all players
    TriggerClientEvent('cbps:timeChanged', -1, hour, minute)
    
    print('[CBPS Menu] Time changed to ' .. hour .. ':' .. minute .. ' by ' .. GetPlayerName(playerId))
end)

-- Client events for weather and time
RegisterNetEvent('cbps:weatherChanged')
AddEventHandler('cbps:weatherChanged', function(weather)
    SetWeatherTypeNow(weather)
    SetWeatherTypePersist(weather)
    SetWeatherTypeNowPersist(weather)
end)

RegisterNetEvent('cbps:timeChanged')
AddEventHandler('cbps:timeChanged', function(hour, minute)
    NetworkOverrideClockTime(hour, minute, 0)
end)
