-- Character Management Client

local savedCharacters = {}
local currentCharacterData = nil
local defaultCharacter = nil

-- Character customization options
local characterOptions = {
    -- Appearance
    face = {
        motherFace = 0,
        fatherFace = 0,
        faceMix = 0.5,
        skinMix = 0.5
    },
    -- Features (0.0 to 1.0)
    features = {
        noseWidth = 0.5,
        noseHeight = 0.5,
        noseLength = 0.5,
        noseBridge = 0.5,
        noseTip = 0.5,
        noseShift = 0.5,
        browHeight = 0.5,
        browWidth = 0.5,
        cheekboneHeight = 0.5,
        cheekboneWidth = 0.5,
        cheeksWidth = 0.5,
        eyes = 0.5,
        lips = 0.5,
        jawWidth = 0.5,
        jawHeight = 0.5,
        chinLength = 0.5,
        chinPosition = 0.5,
        chinWidth = 0.5,
        chinShape = 0.5,
        neckWidth = 0.5
    },
    -- Hair
    hair = {
        style = 0,
        color = 0,
        highlight = 0
    },
    -- Eye color
    eyeColor = 0,
    -- Facial hair
    facialHair = {
        style = -1,
        color = 0,
        opacity = 1.0
    },
    -- Overlays (makeup, blemishes, etc.)
    overlays = {
        blemishes = {index = -1, opacity = 0.0},
        facialHair = {index = -1, color = 0, opacity = 0.0},
        eyebrows = {index = -1, color = 0, opacity = 1.0},
        ageing = {index = -1, opacity = 0.0},
        makeup = {index = -1, opacity = 0.0},
        blush = {index = -1, color = 0, opacity = 0.0},
        complexion = {index = -1, opacity = 0.0},
        sunDamage = {index = -1, opacity = 0.0},
        lipstick = {index = -1, color = 0, opacity = 0.0},
        molesFreckles = {index = -1, opacity = 0.0},
        chestHair = {index = -1, color = 0, opacity = 0.0},
        bodyBlemishes = {index = -1, opacity = 0.0}
    }
}

-- Load saved characters
function LoadSavedCharacters()
    local data = GetResourceKvpString('cbps_saved_characters')
    if data then
        savedCharacters = json.decode(data) or {}
    end
    
    local defaultData = GetResourceKvpString('cbps_default_character')
    if defaultData then
        defaultCharacter = defaultData
    end
end

-- Save characters to storage
function SaveCharactersToStorage()
    local data = json.encode(savedCharacters)
    SetResourceKvp('cbps_saved_characters', data)
end

-- Save current character
function SaveCharacter(characterName)
    if not characterName or characterName == "" then
        ShowNotification('~r~Invalid character name')
        return false
    end
    
    -- Get current ped appearance
    local playerPed = PlayerPedId()
    local characterData = GetCurrentPedAppearance(playerPed)
    
    savedCharacters[characterName] = characterData
    SaveCharactersToStorage()
    
    ShowNotification('~g~Character "' .. characterName .. '" saved!')
    return true
end

-- Load character
function LoadCharacter(characterName)
    if not savedCharacters[characterName] then
        ShowNotification('~r~Character not found')
        return false
    end
    
    local characterData = savedCharacters[characterName]
    ApplyCharacterAppearance(characterData)
    currentCharacterData = characterData
    
    ShowNotification('~g~Character "' .. characterName .. '" loaded!')
    return true
end

-- Set default character
function SetDefaultCharacter(characterName)
    if not savedCharacters[characterName] then
        ShowNotification('~r~Character not found')
        return false
    end
    
    defaultCharacter = characterName
    SetResourceKvp('cbps_default_character', characterName)
    
    ShowNotification('~g~Default character set to "' .. characterName .. '"')
    return true
end

-- Delete character
function DeleteCharacter(characterName)
    if not savedCharacters[characterName] then
        ShowNotification('~r~Character not found')
        return false
    end
    
    savedCharacters[characterName] = nil
    SaveCharactersToStorage()
    
    if defaultCharacter == characterName then
        defaultCharacter = nil
        SetResourceKvp('cbps_default_character', '')
    end
    
    ShowNotification('~g~Character "' .. characterName .. '" deleted!')
    return true
end

-- Get current ped appearance
function GetCurrentPedAppearance(ped)
    local data = {
        model = GetEntityModel(ped),
        -- Face blend
        headBlend = {
            shapeFirst = 0,
            shapeSecond = 0,
            shapeThird = 0,
            skinFirst = 0,
            skinSecond = 0,
            skinThird = 0,
            shapeMix = 0.0,
            skinMix = 0.0,
            thirdMix = 0.0
        },
        -- Hair
        hair = {
            style = GetPedDrawableVariation(ped, 2),
            color = GetPedHairColor(ped),
            highlight = GetPedHairHighlightColor(ped)
        },
        -- Eye color
        eyeColor = GetPedEyeColor(ped),
        -- Face features
        faceFeatures = {},
        -- Head overlays
        headOverlays = {}
    }
    
    -- Get face features
    for i = 0, 19 do
        data.faceFeatures[i] = GetPedFaceFeature(ped, i)
    end
    
    -- Get head overlays
    for i = 0, 12 do
        local success, overlayValue, colourType, firstColour, secondColour, overlayOpacity = GetPedHeadOverlayData(ped, i)
        if success then
            data.headOverlays[i] = {
                index = overlayValue,
                color = firstColour,
                secondColor = secondColour,
                opacity = overlayOpacity
            }
        end
    end
    
    return data
end

-- Apply character appearance
function ApplyCharacterAppearance(data)
    local playerPed = PlayerPedId()
    
    -- Apply model if different
    if data.model and GetEntityModel(playerPed) ~= data.model then
        RequestModel(data.model)
        while not HasModelLoaded(data.model) do
            Citizen.Wait(0)
        end
        SetPlayerModel(PlayerId(), data.model)
        SetModelAsNoLongerNeeded(data.model)
        playerPed = PlayerPedId()
    end
    
    -- Apply head blend
    if data.headBlend then
        SetPedHeadBlendData(playerPed, 
            data.headBlend.shapeFirst or 0,
            data.headBlend.shapeSecond or 0,
            data.headBlend.shapeThird or 0,
            data.headBlend.skinFirst or 0,
            data.headBlend.skinSecond or 0,
            data.headBlend.skinThird or 0,
            data.headBlend.shapeMix or 0.5,
            data.headBlend.skinMix or 0.5,
            data.headBlend.thirdMix or 0.0,
            false
        )
    end
    
    -- Apply hair
    if data.hair then
        SetPedComponentVariation(playerPed, 2, data.hair.style or 0, 0, 0)
        SetPedHairColor(playerPed, data.hair.color or 0, data.hair.highlight or 0)
    end
    
    -- Apply eye color
    if data.eyeColor then
        SetPedEyeColor(playerPed, data.eyeColor)
    end
    
    -- Apply face features
    if data.faceFeatures then
        for i = 0, 19 do
            if data.faceFeatures[i] then
                SetPedFaceFeature(playerPed, i, data.faceFeatures[i])
            end
        end
    end
    
    -- Apply head overlays
    if data.headOverlays then
        for i = 0, 12 do
            if data.headOverlays[i] then
                local overlay = data.headOverlays[i]
                SetPedHeadOverlay(playerPed, i, overlay.index or 0, overlay.opacity or 0.0)
                if overlay.color then
                    SetPedHeadOverlayColor(playerPed, i, 1, overlay.color, overlay.secondColor or 0)
                end
            end
        end
    end
end

-- Open character creator
function OpenCharacterCreator()
    ShowNotification('~b~Opening Character Creator...')
    -- Simple character randomization for now
    local playerPed = PlayerPedId()
    
    -- Randomize appearance
    SetPedRandomComponentVariation(playerPed, false)
    SetPedRandomProps(playerPed)
    
    ShowNotification('~g~Character randomized! Save it from the menu.')
end

-- Event: Save character from menu
RegisterNetEvent('cbps:saveCharacterPrompt')
AddEventHandler('cbps:saveCharacterPrompt', function()
    DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP8", "", "Enter character name", "", "", "", 32)
    while UpdateOnscreenKeyboard() == 0 do
        Citizen.Wait(0)
    end
    
    if GetOnscreenKeyboardResult() then
        local characterName = GetOnscreenKeyboardResult()
        SaveCharacter(characterName)
    end
end)

-- Event: Load character from menu
RegisterNetEvent('cbps:loadCharacterPrompt')
AddEventHandler('cbps:loadCharacterPrompt', function()
    if next(savedCharacters) == nil then
        ShowNotification('~r~No saved characters')
        return
    end
    
    -- Show list of characters
    local characterList = {}
    for name, _ in pairs(savedCharacters) do
        table.insert(characterList, name)
    end
    
    ShowNotification('~b~Saved characters:')
    for i, name in ipairs(characterList) do
        ShowNotification('~y~' .. i .. '. ' .. name)
    end
end)

-- Load default character on spawn
AddEventHandler('playerSpawned', function()
    LoadSavedCharacters()
    
    if defaultCharacter and savedCharacters[defaultCharacter] then
        Citizen.Wait(1000) -- Wait for player to spawn
        LoadCharacter(defaultCharacter)
    end
end)

-- Initialize
Citizen.CreateThread(function()
    Citizen.Wait(2000)
    LoadSavedCharacters()
end)

-- Exports
exports('SaveCharacter', SaveCharacter)
exports('LoadCharacter', LoadCharacter)
exports('SetDefaultCharacter', SetDefaultCharacter)
exports('DeleteCharacter', DeleteCharacter)
exports('GetSavedCharacters', function() return savedCharacters end)
exports('OpenCharacterCreator', OpenCharacterCreator)
