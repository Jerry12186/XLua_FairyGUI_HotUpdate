Debug = {}

Debug.debugmode = true

function Debug.log(info)
    if Debug.debugmode then
        print(info)
    end
end