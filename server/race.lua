-- Server Race Management

local races = {}
local playerRaces = {}
local raceResults = {}
local nextRaceId = 1

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
