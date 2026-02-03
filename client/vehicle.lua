-- Vehicle Options
local currentVehicle = nil

-- Vehicle spawning event
RegisterNetEvent('cbps:spawnVehicle')
AddEventHandler('cbps:spawnVehicle', function(model)
    SpawnVehicle(model)
end)

function SpawnVehicle(model)
    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    local heading = GetEntityHeading(playerPed)
    
    RequestModel(model)
    while not HasModelLoaded(model) do
        Citizen.Wait(0)
    end
    
    -- Delete old vehicle if exists
    if currentVehicle and DoesEntityExist(currentVehicle) then
        DeleteEntity(currentVehicle)
    end
    
    -- Spawn new vehicle
    local vehicle = CreateVehicle(model, coords.x, coords.y, coords.z, heading, true, false)
    SetPedIntoVehicle(playerPed, vehicle, -1)
    SetEntityAsNoLongerNeeded(vehicle)
    SetModelAsNoLongerNeeded(model)
    
    currentVehicle = vehicle
    ShowNotification('~g~Vehicle spawned: ' .. model)
end

function RepairVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        SetVehicleFixed(vehicle)
        SetVehicleDeformationFixed(vehicle)
        SetVehicleUndriveable(vehicle, false)
        SetVehicleEngineOn(vehicle, true, true)
        ShowNotification('~g~Vehicle repaired')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

function CleanVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        SetVehicleDirtLevel(vehicle, 0.0)
        ShowNotification('~g~Vehicle cleaned')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

function FlipVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        local coords = GetEntityCoords(vehicle)
        local heading = GetEntityHeading(vehicle)
        SetEntityCoords(vehicle, coords.x, coords.y, coords.z + 2.0, 0, 0, 0, false)
        SetEntityRotation(vehicle, 0.0, 0.0, heading, 2, true)
        ShowNotification('~g~Vehicle flipped')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

function BoostVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        SetVehicleForwardSpeed(vehicle, 50.0)
        ShowNotification('~g~Vehicle boosted')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

local vehicleInvincible = false
function ToggleVehicleInvincible()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        vehicleInvincible = not vehicleInvincible
        SetEntityInvincible(vehicle, vehicleInvincible)
        SetVehicleCanBeVisiblyDamaged(vehicle, not vehicleInvincible)
        ShowNotification(vehicleInvincible and '~g~Vehicle Invincible: ON' or '~r~Vehicle Invincible: OFF')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

function DeleteCurrentVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        DeleteEntity(vehicle)
        ShowNotification('~g~Vehicle deleted')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

function MaxUpgradeVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        -- Engine
        SetVehicleMod(vehicle, 11, GetNumVehicleMods(vehicle, 11) - 1, false)
        -- Brakes
        SetVehicleMod(vehicle, 12, GetNumVehicleMods(vehicle, 12) - 1, false)
        -- Transmission
        SetVehicleMod(vehicle, 13, GetNumVehicleMods(vehicle, 13) - 1, false)
        -- Suspension
        SetVehicleMod(vehicle, 15, GetNumVehicleMods(vehicle, 15) - 1, false)
        -- Armor
        SetVehicleMod(vehicle, 16, GetNumVehicleMods(vehicle, 16) - 1, false)
        -- Turbo
        ToggleVehicleMod(vehicle, 18, true)
        
        ShowNotification('~g~Vehicle fully upgraded')
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end

local rainbowVehicle = false
function ToggleRainbowVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)
    
    if vehicle and vehicle ~= 0 then
        rainbowVehicle = not rainbowVehicle
        ShowNotification(rainbowVehicle and '~g~Rainbow Mode: ON' or '~r~Rainbow Mode: OFF')
        
        if rainbowVehicle then
            Citizen.CreateThread(function()
                local colors = {
                    {255, 0, 0},    -- Red
                    {255, 165, 0},  -- Orange
                    {255, 255, 0},  -- Yellow
                    {0, 255, 0},    -- Green
                    {0, 0, 255},    -- Blue
                    {75, 0, 130},   -- Indigo
                    {238, 130, 238} -- Violet
                }
                local colorIndex = 1
                
                while rainbowVehicle do
                    Citizen.Wait(100)
                    local veh = GetVehiclePedIsIn(playerPed, false)
                    if veh and veh ~= 0 then
                        local color = colors[colorIndex]
                        SetVehicleCustomPrimaryColour(veh, color[1], color[2], color[3])
                        SetVehicleCustomSecondaryColour(veh, color[1], color[2], color[3])
                        colorIndex = colorIndex + 1
                        if colorIndex > #colors then
                            colorIndex = 1
                        end
                    else
                        rainbowVehicle = false
                    end
                end
            end)
        end
    else
        ShowNotification('~r~You are not in a vehicle')
    end
end
