-- Voice & Radio Control (pma-voice and pma-radio integration)

local currentVoiceRange = 2 -- Default to index 2 (5.0 meters)
local radioFrequency = 0.0

function SetVoiceRange(range)
    exports['pma-voice']:setVoiceProperty('range', range)
    ShowNotification('~b~Voice range set to: ' .. range .. 'm')
end

function SetRadioFrequency(frequency)
    if frequency and frequency >= Config.Radio.MinFrequency and frequency <= Config.Radio.MaxFrequency then
        radioFrequency = frequency
        exports['pma-radio']:SetRadioFrequency(frequency)
        ShowNotification('~b~Radio frequency set to: ' .. frequency)
    else
        ShowNotification('~r~Invalid frequency! Range: ' .. Config.Radio.MinFrequency .. ' - ' .. Config.Radio.MaxFrequency)
    end
end

function TurnOffRadio()
    if radioFrequency > 0 then
        exports['pma-radio']:SetRadioFrequency(0)
        radioFrequency = 0
        ShowNotification('~r~Radio: OFF')
    else
        ShowNotification('~y~Radio is already off')
    end
end

-- Keybind for voice range (ALT key by default)
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        
        -- Cycle voice range with ALT
        if IsControlJustReleased(0, 19) then -- ALT
            currentVoiceRange = currentVoiceRange + 1
            if currentVoiceRange > #Config.Voice.Ranges then
                currentVoiceRange = 1
            end
            
            local range = Config.Voice.Ranges[currentVoiceRange]
            exports['pma-voice']:setVoiceProperty('range', range)
            ShowNotification('~b~Voice range: ' .. range .. 'm')
        end
    end
end)
