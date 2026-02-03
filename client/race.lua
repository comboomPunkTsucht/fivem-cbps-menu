-- Race Functions Client

local inRace = false
local isCreatingRace = false
local raceCheckpoints = {}
local currentCheckpoint = 1
local raceStartTime = 0
local checkpointBlips = {}
local checkpointMarkers = {}

-- Event: Race created
RegisterNetEvent('cbps:raceCreated')
AddEventHandler('cbps:raceCreated', function(raceId)
    ShowNotification('~g~Race created! ID: ' .. raceId)
    ShowNotification('~y~Add checkpoints and start the race')
    isCreatingRace = true
end)

-- Event: Joined race
RegisterNetEvent('cbps:joinedRace')
AddEventHandler('cbps:joinedRace', function(raceId, checkpoints)
    inRace = true
    raceCheckpoints = checkpoints
    currentCheckpoint = 1
    CreateCheckpointBlips()
    ShowNotification('~g~Joined race! ID: ' .. raceId)
end)

-- Event: Left race
RegisterNetEvent('cbps:leftRace')
AddEventHandler('cbps:leftRace', function()
    inRace = false
    isCreatingRace = false
    raceCheckpoints = {}
    currentCheckpoint = 1
    ClearCheckpointBlips()
    ShowNotification('~r~Left race')
end)

-- Event: Race started
RegisterNetEvent('cbps:raceStarted')
AddEventHandler('cbps:raceStarted', function(countdown)
    ShowNotification('~g~Race starting in ' .. countdown .. ' seconds!')
    
    -- Countdown
    Citizen.CreateThread(function()
        for i = countdown, 1, -1 do
            Citizen.Wait(1000)
            SetTextFont(0)
            SetTextProportional(1)
            SetTextScale(2.0, 2.0)
            SetTextColour(255, 255, 0, 255)
            SetTextDropshadow(0, 0, 0, 0, 255)
            SetTextEdge(1, 0, 0, 0, 255)
            SetTextDropShadow()
            SetTextOutline()
            SetTextEntry('STRING')
            AddTextComponentString(tostring(i))
            DrawText(0.5, 0.4)
        end
        
        Citizen.Wait(1000)
        SetTextFont(0)
        SetTextProportional(1)
        SetTextScale(2.0, 2.0)
        SetTextColour(0, 255, 0, 255)
        SetTextDropshadow(0, 0, 0, 0, 255)
        SetTextEdge(1, 0, 0, 0, 255)
        SetTextDropShadow()
        SetTextOutline()
        SetTextEntry('STRING')
        AddTextComponentString('GO!')
        DrawText(0.5, 0.4)
        
        raceStartTime = GetGameTimer()
        MonitorRaceProgress()
    end)
end)

-- Event: Race finished
RegisterNetEvent('cbps:raceFinished')
AddEventHandler('cbps:raceFinished', function(position, time)
    ShowNotification('~g~Race finished! Position: ' .. position .. ' Time: ' .. FormatTime(time))
    inRace = false
    ClearCheckpointBlips()
end)

-- Event: Checkpoint reached
RegisterNetEvent('cbps:checkpointReached')
AddEventHandler('cbps:checkpointReached', function(checkpointNum)
    ShowNotification('~b~Checkpoint ' .. checkpointNum .. ' reached!')
end)

-- Add checkpoint at current location
RegisterNetEvent('cbps:addCheckpoint')
AddEventHandler('cbps:addCheckpoint', function()
    if not isCreatingRace then
        ShowNotification('~r~You need to create a race first')
        return
    end
    
    if #raceCheckpoints >= Config.Race.MaxCheckpoints then
        ShowNotification('~r~Maximum checkpoints reached')
        return
    end
    
    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    
    table.insert(raceCheckpoints, {x = coords.x, y = coords.y, z = coords.z})
    TriggerServerEvent('cbps:addRaceCheckpoint', coords)
    ShowNotification('~g~Checkpoint added: ' .. #raceCheckpoints)
    
    -- Create marker
    local blip = AddBlipForCoord(coords.x, coords.y, coords.z)
    SetBlipSprite(blip, 1)
    SetBlipDisplay(blip, 4)
    SetBlipScale(blip, 0.8)
    SetBlipColour(blip, 5)
    SetBlipAsShortRange(blip, true)
    BeginTextCommandSetBlipName('STRING')
    AddTextComponentString('Checkpoint ' .. #raceCheckpoints)
    EndTextCommandSetBlipName(blip)
    
    table.insert(checkpointBlips, blip)
end)

-- Clear all checkpoints
RegisterNetEvent('cbps:clearCheckpoints')
AddEventHandler('cbps:clearCheckpoints', function()
    raceCheckpoints = {}
    ClearCheckpointBlips()
    TriggerServerEvent('cbps:clearRaceCheckpoints')
    ShowNotification('~y~All checkpoints cleared')
end)

function CreateCheckpointBlips()
    ClearCheckpointBlips()
    
    for i, checkpoint in ipairs(raceCheckpoints) do
        local blip = AddBlipForCoord(checkpoint.x, checkpoint.y, checkpoint.z)
        SetBlipSprite(blip, 1)
        SetBlipDisplay(blip, 4)
        SetBlipScale(blip, 0.8)
        SetBlipColour(blip, i == 1 and 2 or 5)
        SetBlipAsShortRange(blip, true)
        BeginTextCommandSetBlipName('STRING')
        AddTextComponentString('Checkpoint ' .. i)
        EndTextCommandSetBlipName(blip)
        
        table.insert(checkpointBlips, blip)
    end
end

function ClearCheckpointBlips()
    for _, blip in ipairs(checkpointBlips) do
        RemoveBlip(blip)
    end
    checkpointBlips = {}
end

function MonitorRaceProgress()
    Citizen.CreateThread(function()
        while inRace and currentCheckpoint <= #raceCheckpoints do
            Citizen.Wait(0)
            
            local playerPed = PlayerPedId()
            local playerCoords = GetEntityCoords(playerPed)
            local checkpoint = raceCheckpoints[currentCheckpoint]
            
            -- Draw checkpoint marker
            DrawMarker(1, checkpoint.x, checkpoint.y, checkpoint.z - 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 
                Config.Race.CheckpointRadius * 2, Config.Race.CheckpointRadius * 2, 2.0, 0, 255, 0, 100, false, true, 2, false, nil, nil, false)
            
            -- Check if player reached checkpoint
            local distance = #(playerCoords - vector3(checkpoint.x, checkpoint.y, checkpoint.z))
            if distance < Config.Race.CheckpointRadius then
                TriggerServerEvent('cbps:reachedCheckpoint', currentCheckpoint)
                currentCheckpoint = currentCheckpoint + 1
                
                if currentCheckpoint > #raceCheckpoints then
                    -- Race finished
                    local finishTime = GetGameTimer() - raceStartTime
                    TriggerServerEvent('cbps:finishRace', finishTime)
                end
            end
            
            -- Display current time
            local currentTime = GetGameTimer() - raceStartTime
            SetTextFont(0)
            SetTextProportional(1)
            SetTextScale(0.5, 0.5)
            SetTextColour(255, 255, 255, 255)
            SetTextDropshadow(0, 0, 0, 0, 255)
            SetTextEdge(1, 0, 0, 0, 255)
            SetTextDropShadow()
            SetTextOutline()
            SetTextEntry('STRING')
            AddTextComponentString('Time: ' .. FormatTime(currentTime) .. '\nCheckpoint: ' .. currentCheckpoint .. '/' .. #raceCheckpoints)
            DrawText(0.85, 0.1)
        end
    end)
end

function FormatTime(ms)
    local seconds = math.floor(ms / 1000)
    local minutes = math.floor(seconds / 60)
    seconds = seconds % 60
    local milliseconds = ms % 1000
    
    return string.format('%02d:%02d.%03d', minutes, seconds, milliseconds)
end

-- Export functions
exports('IsInRace', function()
    return inRace
end)

exports('IsCreatingRace', function()
    return isCreatingRace
end)
