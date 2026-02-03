-- Server Player Management

-- Admin check
function IsPlayerAdmin(playerId)
    if not Config.AdminOnly then
        return true
    end
    
    local identifiers = GetPlayerIdentifiers(playerId)
    for _, identifier in pairs(identifiers) do
        for _, admin in pairs(Config.Admins) do
            if identifier == admin then
                return true
            end
        end
    end
    
    return false
end

-- Export the function
exports('IsPlayerAdmin', IsPlayerAdmin)
