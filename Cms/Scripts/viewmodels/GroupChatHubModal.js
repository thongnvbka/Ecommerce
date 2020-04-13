var groupChatCurrentPage = 1;
var groupChatPageSize = 10;
var groupChatTotalRows = 0;
var imageExtension = [".jpg", ".png", ".gif", ".jpeg"];
//var groupChat;
var emoticonLibs = {
    ":)": "&nbsp;<img src='/content/images/advance_smiles/smile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;' />&nbsp;",
    ":(": "&nbsp;<img src='/content/images/advance_smiles/sadsmile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":D": "&nbsp;<img src='/content/images/advance_smiles/bigsmile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":d": "&nbsp;<img src='/content/images/advance_smiles/bigsmile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "8)": "&nbsp;<img src='/content/images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "b)": "&nbsp;<img src='/content/images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "B)": "&nbsp;<img src='/content/images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(cool)": "&nbsp;<img src='/content/images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(wink)": "&nbsp;<img src='/content/images/advance_smiles/wink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ";)": "&nbsp;<img src='/content/images/advance_smiles/wink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":o": "&nbsp;<img src='/content/images/advance_smiles/surprised_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":O": "&nbsp;<img src='/content/images/advance_smiles/surprised_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ";(": "&nbsp;<img src='/content/images/advance_smiles/crying_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(sweat)": "&nbsp;<img src='/content/images/advance_smiles/sweating_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(:|": "&nbsp;<img src='/content/images/advance_smiles/sweating_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":|": "&nbsp;<img src='/content/images/advance_smiles/speechless_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":*": "&nbsp;<img src='/content/images/advance_smiles/kiss_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(kiss)": "&nbsp;<img src='/content/images/advance_smiles/kiss_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":p": "&nbsp;<img src='/content/images/advance_smiles/tongueout_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":P": "&nbsp;<img src='/content/images/advance_smiles/tongueout_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(yn)": "&nbsp;<img src='/content/images/advance_smiles/fingerscrossed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(blush)": "&nbsp;<img src='/content/images/advance_smiles/blushing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":$": "&nbsp;<img src='/content/images/advance_smiles/blushing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":\">": "&nbsp;<img src='/content/images/advance_smiles/blushing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":^)": "&nbsp;<img src='/content/images/advance_smiles/wondering_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "|)": "&nbsp;<img src='/content/images/advance_smiles/sleepy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(snooze)": "&nbsp;<img src='/content/images/advance_smiles/sleepy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "|(": "&nbsp;<img src='/content/images/advance_smiles/dull_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(inlove)": "&nbsp;<img src='/content/images/advance_smiles/inlove_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(grin)": "&nbsp;<img src='/content/images/advance_smiles/evilgrin_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "]:)": "&nbsp;<img src='/content/images/advance_smiles/evilgrin_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ">:)": "&nbsp;<img src='/content/images/advance_smiles/evilgrin_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(yawn)": "&nbsp;<img src='/content/images/advance_smiles/yawning_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(puke)": "&nbsp;<img src='/content/images/advance_smiles/puking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":&": "&nbsp;<img src='/content/images/advance_smiles/puking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(doh)": "&nbsp;<img src='/content/images/advance_smiles/doh_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":@": "&nbsp;<img src='/content/images/advance_smiles/angry_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "x(": "&nbsp;<img src='/content/images/advance_smiles/angry_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(wasntme)": "&nbsp;<img src='/content/images/advance_smiles/itwasntme_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(party)": "&nbsp;<img src='/content/images/advance_smiles/party_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(facepalm)": "&nbsp;<img src='/content/images/advance_smiles/facepalm_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":S": "&nbsp;<img src='/content/images/advance_smiles/worried_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":s": "&nbsp;<img src='/content/images/advance_smiles/worried_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(mm)": "&nbsp;<img src='/content/images/advance_smiles/mmm_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "8|": "&nbsp;<img src='/content/images/advance_smiles/nerd_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "B|": "&nbsp;<img src='/content/images/advance_smiles/nerd_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "b|": "&nbsp;<img src='/content/images/advance_smiles/nerd_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":x": "&nbsp;<img src='/content/images/advance_smiles/lipssealed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":X": "&nbsp;<img src='/content/images/advance_smiles/lipssealed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(hi)": "&nbsp;<img src='/content/images/advance_smiles/hi_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(wave)": "&nbsp;<img src='/content/images/advance_smiles/hi_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(devil)": "&nbsp;<img src='/content/images/advance_smiles/devil_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(angle)": "&nbsp;<img src='/content/images/advance_smiles/angle_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(envy)": "&nbsp;<img src='/content/images/advance_smiles/envy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(wait)": "&nbsp;<img src='/content/images/advance_smiles/wait_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(bear)": "&nbsp;<img src='/content/images/advance_smiles/hug_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(hug)": "&nbsp;<img src='/content/images/advance_smiles/hug_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(makeup)": "&nbsp;<img src='/content/images/advance_smiles/makup_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(kate)": "&nbsp;<img src='/content/images/advance_smiles/makup_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(moustache)": "&nbsp;<img src='/content/images/advance_smiles/moustache_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(giggle)": "&nbsp;<img src='/content/images/advance_smiles/giggle_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(chuckle)": "&nbsp;<img src='/content/images/advance_smiles/makup_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(clap)": "&nbsp;<img src='/content/images/advance_smiles/clap_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":?": "&nbsp;<img src='/content/images/advance_smiles/think_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(think)": "&nbsp;<img src='/content/images/advance_smiles/think_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(bow)": "&nbsp;<img src='/content/images/advance_smiles/bow_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(rofl)": "&nbsp;<img src='/content/images/advance_smiles/rofl_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(whew)": "&nbsp;<img src='/content/images/advance_smiles/whew_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(highfive)": "&nbsp;<img src='/content/images/advance_smiles/highfive_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(happy)": "&nbsp;<img src='/content/images/advance_smiles/happy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(smirk)": "&nbsp;<img src='/content/images/advance_smiles/smirk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(nod)": "&nbsp;<img src='/content/images/advance_smiles/nod_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(shake)": "&nbsp;<img src='/content/images/advance_smiles/shake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(emo)": "&nbsp;<img src='/content/images/advance_smiles/emo_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(waiting)": "&nbsp;<img src='/content/images/advance_smiles/waiting_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(y)": "&nbsp;<img src='/content/images/advance_smiles/yes_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(Y)": "&nbsp;<img src='/content/images/advance_smiles/yes_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(ok)": "&nbsp;<img src='/content/images/advance_smiles/yes_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(n)": "&nbsp;<img src='/content/images/advance_smiles/no_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(N)": "&nbsp;<img src='/content/images/advance_smiles/no_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(handshake)": "&nbsp;<img src='/content/images/advance_smiles/handshake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(h)": "&nbsp;<img src='/content/images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(H)": "&nbsp;<img src='/content/images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(l)": "&nbsp;<img src='/content/images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(L)": "&nbsp;<img src='/content/images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(lalala)": "&nbsp;<img src='/content/images/advance_smiles/lalala_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(tmi)": "&nbsp;<img src='/content/images/advance_smiles/tmi_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(heidy)": "&nbsp;<img src='/content/images/advance_smiles/heidy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(f)": "&nbsp;<img src='/content/images/advance_smiles/flower_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(F)": "&nbsp;<img src='/content/images/advance_smiles/flower_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(rain)": "&nbsp;<img src='/content/images/advance_smiles/rain_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(sun)": "&nbsp;<img src='/content/images/advance_smiles/sunshine_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(music)": "&nbsp;<img src='/content/images/advance_smiles/music_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(coffee)": "&nbsp;<img src='/content/images/advance_smiles/coffee_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(pizza)": "&nbsp;<img src='/content/images/advance_smiles/pizza_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(cash)": "&nbsp;<img src='/content/images/advance_smiles/cash_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(flex)": "&nbsp;<img src='/content/images/advance_smiles/muscle_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(cake)": "&nbsp;<img src='/content/images/advance_smiles/cake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(^)": "&nbsp;<img src='/content/images/advance_smiles/cake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(beer)": "&nbsp;<img src='/content/images/advance_smiles/beer_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(d)": "&nbsp;<img src='/content/images/advance_smiles/drink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(D)": "&nbsp;<img src='/content/images/advance_smiles/drink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(dance)": "&nbsp;<img src='/content/images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "\o/": "&nbsp;<img src='/content/images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "\:D/": "&nbsp;<img src='/content/images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "\:d/": "&nbsp;<img src='/content/images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(ninja)": "&nbsp;<img src='/content/images/advance_smiles/ninja_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(*)": "&nbsp;<img src='/content/images/advance_smiles/star_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(tumbleweed)": "&nbsp;<img src='/content/images/advance_smiles/tumbleweed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(bandit)": "&nbsp;<img src='/content/images/advance_smiles/bandit_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(rock)": "&nbsp;<img src='/content/images/advance_smiles/rock_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(banghead)": "&nbsp;<img src='/content/images/advance_smiles/banghead_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(punch)": "&nbsp;<img src='/content/images/advance_smiles/punch_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(talk)": "&nbsp;<img src='/content/images/advance_smiles/talk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(swear)": "&nbsp;<img src='/content/images/advance_smiles/swear_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(idea)": "&nbsp;<img src='/content/images/advance_smiles/idea_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(talktothehand)": "&nbsp;<img src='/content/images/advance_smiles/talktothehand_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(u)": "&nbsp;<img src='/content/images/advance_smiles/brokenheart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(U)": "&nbsp;<img src='/content/images/advance_smiles/brokenheart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(fistbump)": "&nbsp;<img src='/content/images/advance_smiles/fistbump_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(pray)": "&nbsp;<img src='/content/images/advance_smiles/pray_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(poke)": "&nbsp;<img src='/content/images/advance_smiles/poke_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(victory)": "&nbsp;<img src='/content/images/advance_smiles/victory_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(celebrate)": "&nbsp;<img src='/content/images/advance_smiles/celebrate_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(brb)": "&nbsp;<img src='/content/images/advance_smiles/brb_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(wfh)": "&nbsp;<img src='/content/images/advance_smiles/wfh_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(mbp)": "&nbsp;<img src='/content/images/advance_smiles/phone_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(ph)": "&nbsp;<img src='/content/images/advance_smiles/phone_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(call)": "&nbsp;<img src='/content/images/advance_smiles/call_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(e)": "&nbsp;<img src='/content/images/advance_smiles/mail_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(m)": "&nbsp;<img src='/content/images/advance_smiles/mail_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(film)": "&nbsp;<img src='/content/images/advance_smiles/movie_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(movie)": "&nbsp;<img src='/content/images/advance_smiles/movie_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(o)": "&nbsp;<img src='/content/images/advance_smiles/time_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(O)": "&nbsp;<img src='/content/images/advance_smiles/time_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(time)": "&nbsp;<img src='/content/images/advance_smiles/time_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(sheep)": "&nbsp;<img src='/content/images/advance_smiles/sheep_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(cat)": "&nbsp;<img src='/content/images/advance_smiles/cat_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    ":3": "&nbsp;<img src='/content/images/advance_smiles/cat_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(bike)": "&nbsp;<img src='/content/images/advance_smiles/bike_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(dog)": "&nbsp;<img src='/content/images/advance_smiles/dog_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(soccer)": "&nbsp;<img src='/content/images/advance_smiles/soccer_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(monkey)": "&nbsp;<img src='/content/images/advance_smiles/monkey_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(bomb)": "&nbsp;<img src='/content/images/advance_smiles/bomb_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(man)": "&nbsp;<img src='/content/images/advance_smiles/man_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(woman)": "&nbsp;<img src='/content/images/advance_smiles/woman_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(drunk)": "&nbsp;<img src='/content/images/advance_smiles/drunk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(smoking)": "&nbsp;<img src='/content/images/advance_smiles/smoking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(ci)": "&nbsp;<img src='/content/images/advance_smiles/smoking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(smoke)": "&nbsp;<img src='/content/images/advance_smiles/smoking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(gottarun)": "&nbsp;<img src='/content/images/advance_smiles/gottarun_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(stop)": "&nbsp;<img src='/content/images/advance_smiles/stop_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(toivo)": "&nbsp;<img src='/content/images/advance_smiles/toivo_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(bug)": "&nbsp;<img src='/content/images/advance_smiles/bug_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(poolparty)": "&nbsp;<img src='/content/images/advance_smiles/poolparty_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(snail)": "&nbsp;<img src='/content/images/advance_smiles/snail_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(goodluck)": "&nbsp;<img src='/content/images/advance_smiles/goodluck_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(island)": "&nbsp;<img src='/content/images/advance_smiles/island_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(umbrella)": "&nbsp;<img src='/content/images/advance_smiles/umbrella_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(rainbow)": "&nbsp;<img src='/content/images/advance_smiles/rainbow_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(canyoutalk)": "&nbsp;<img src='/content/images/advance_smiles/canyoutalk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(camera)": "&nbsp;<img src='/content/images/advance_smiles/camera_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(plane)": "&nbsp;<img src='/content/images/advance_smiles/plane_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(car)": "&nbsp;<img src='/content/images/advance_smiles/car_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(computer)": "&nbsp;<img src='/content/images/advance_smiles/computer_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(games)": "&nbsp;<img src='/content/images/advance_smiles/games_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(gift)": "&nbsp;<img src='/content/images/advance_smiles/gift_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(holdon)": "&nbsp;<img src='/content/images/advance_smiles/holdon_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(letsmeet)": "&nbsp;<img src='/content/images/advance_smiles/letsmeet_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(confidential)": "&nbsp;<img src='/content/images/advance_smiles/confidential_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(whatsgoingon)": "&nbsp;<img src='/content/images/advance_smiles/whatsgoingon_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(malthe)": "&nbsp;<img src='/content/images/advance_smiles/malthe_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(tauri)": "&nbsp;<img src='/content/images/advance_smiles/dull_tauri_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(zilmer)": "&nbsp;<img src='/content/images/advance_smiles/priidu_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
    "(oliver)": "&nbsp;<img src='/content/images/advance_smiles/oliver_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
};

function smilesFilter(html) {
    return html.replace(/(\([a-zA-Z0-9]*\)|[a-zA-Z0-9\:\|\)\(\;\(\]\>\<\[\*]*)*/gi, function (a, b) {
        return emoticonLibs[b] || a;
    });
};

function GroupChatHubModalViewModel(groupId, pageTitle, option, callback) {
    var groupChat;
    var self = this;

    self.groupId = groupId;
    self.pageTitle = pageTitle;
    self.callback = callback;
    self.pageUrl = typeof (window.pageUrl) != "undefined" ? window.pageUrl : "";

    self.option = {
        commentListUserTag: [],
        type: 0,
        isShowNotify: false,
        isShowCommentBox: true,
        isCommentFocus: false,
        listUserTag: []
    }

    self.option = $.extend(self.option, option);

    var getUploadItem = function (item) {
        if (imageExtension.indexOf(item.Extension) > -1) {
            return "<img src='/images/ " + item.AttachmentPath + "_50x50_1.jpg' alt='" + item.AttachmentName + "' />&nbsp;";
        } else {
            return "<div class='attachment-item attachment-" + item.Extension.replace(".", "") + "'><i class='fa fa-" + item.Extension.replace(".", "") + "'></i><span style='text-indent: -9999px; display: block'>1</span></div>&nbsp;";
        }
    };

    function setEditorFocus(id) {
        $(id).henryEditor("setFocus");
    };

    function initEditor() {
        // Khởi tạo ô chat
        if (self.option.type === 0 && typeof (CKEDITOR) != 'undefined')
            CKEDITOR.disableAutoInline = true;

        //var element = "#commentBox_" + self.groupId;
        var url = self.option.type === 0 ? "/Upload/UploadAttachment" : "/CustomerUpload/UploadAttachment";

        $("#commentBoxModal").henryEditor({
            fileUpload: {
                url: url,
                callback: function (result) {
                    var arrayAttachment = [];
                    _.each(result, function (item) {
                        arrayAttachment.push(item);
                    });

                    self.listAttachment(arrayAttachment);

                    setTimeout(function () {
                        //$("#sendMessage_" + self.groupId).trigger("click");
                        self.sendMessage();
                    }, 100);
                },
                selectImageLabel: resources.common.message.selectFiles,
                allowFilesType: ["png", "gif", "jpeg", "jpg", "rar", "zip"]
            },
            suggestSource: self.option.listUserTag,
            onKeyPress: function (key) {
                if (key === 13) {
                    self.sendMessage();
                    //$("#sendMessage_" + self.groupId).trigger("click");
                    return false;
                }
            }
        });
    };

    function loopListComments(listComments) {
        var userInfo = window.currentUser;
        _.each(listComments, function (item) {
            item.IsMine = item.UserId === userInfo.UserId;
            item.Class = item.IsMine ? 'media mine' : 'media';
            item.IsCustomer = item.Type === 1;
            item.Time = moment(item.SentTime).fromNow();
            item.NumberOfReplies = ko.observable(item.NumberOfReplies);
            item.Like = ko.observable(item.Like);
            item.Liked = ko.observable(item.Liked);
            item.LikeText = ko.computed(function () {
                return item.Liked() ? resources.common.label.unLike : resources.common.label.like;
            });
            item.IsShowReply = ko.observable(false);
            item.ListReplies = ko.observableArray([]);
            item.IsSending = ko.observable(false);
            item.IsLoadingReply = ko.observable(false);
            item.IsLoadingMoreReply = ko.observable(false);
            item.IsShowLoadmoreReply = ko.computed(function () {
                return item.ListReplies().length < item.NumberOfReplies() && item.NumberOfReplies() > groupChatPageSize && !item.IsLoadingReply();
            });

            if (item.AttachmentCount != null && item.AttachmentCount > 0) {
                var message = $.parseJSON(item.Content);

                // Nếu lớn hơn 5 file hiển thị dạng nhóm.
                var html = '';
                var attachmentsHtml;
                if (item.AttachmentCount > 5) {
                    attachmentsHtml = '';
                    html = '<div class="wrapper-attachment-compress">\
                        <span class="attachment-count" id="attachment-count-' + item.Id + '">{fileCount}</span>\
                        <span> {file}</span>\
                        <a href="javascript://" class="show-all-attachment" data-target="#attachment-'+ item.Id + '">{all}</a>\
                        <ul class="wrapper-list-attachment media-list" id="attachment-'+ item.Id + '">{listAttachment}<ul>\
                        </div>';

                    _.each(message, function (attachment) {
                        attachmentsHtml += '<li class="media" id="attachment-item-' + attachment.Id + '">\
                            <div class="media-left">\
                                <a href="javascript://">'+ getUploadItem(attachment) + '</a></div>\
                            <div class="media-body">\
                            <h4 class="media-heading attachment-name">' + attachment.AttachmentName + '</h4>\
                            <ul class="wrapper-file-attach-options">\
                                <li><a href="/GroupChat/DownloadAttachment?attachmentId={attachmentId}&userType={userType}&groupChatId={groupChatId}" class="pr-icon-left download" data-id="' + attachment.Id + '" data-chat-id="' + item.Id + '"><i class="fa fa-download"></i><span>{download}</span></a></li>\
                                {removeButton}\
                            </ul>\
                        </div>';

                        if (window.currentUser.UserId === attachment.UploaderId) {
                            attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, '<li><a href="javascript://" class="pr-icon-left remove" data-id="' + attachment.Id + '" data-chat-id="' + item.Id + '"><i class="fa fa-trash"></i><span>{deletes}</span></a></li>');
                        } else {
                            attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, "");
                        }

                        attachmentsHtml = attachmentsHtml.replace(/{download}/ig, resources.common.label.download);
                        attachmentsHtml = attachmentsHtml.replace(/{attachmentId}/ig, attachment.Id);
                        attachmentsHtml = attachmentsHtml.replace(/{deletes}/ig, resources.common.label.deletes);
                        attachmentsHtml = attachmentsHtml.replace(/{userType}/ig, self.option.type);
                        attachmentsHtml = attachmentsHtml.replace(/{groupChatId}/ig, self.groupId);
                    });

                    html = html.replace(/{fileCount}/ig, item.AttachmentCount);
                    html = html.replace(/{listAttachment}/ig, attachmentsHtml);
                } else {
                    attachmentsHtml = '';
                    html = '<ul class="wrapper-list-attachment media-list">{listAttachment}</ul>';
                    _.each(message, function (attachment) {
                        attachmentsHtml += '<li class="media" id="attachment-item-' + attachment.Id + '">\
                            {mediaLeft}\
                            <div class="media-body">\
                            <h4 class="media-heading">' + attachment.AttachmentName + '</h4>\
                            <ul class="wrapper-file-attach-options">\
                                <li><a href="/GroupChat/DownloadAttachment?attachmentId={attachmentId}&userType={userType}&groupChatId={groupChatId}" class="pr-icon-left download" data-id="' + attachment.Id + '" data-chat-id="' + item.Id + '"><i class="fa fa-download"></i><span>{download}</span></a></li>\
                                {removeButton}\
                            </ul>\
                        </div>\
                        {mediaRight}';

                        if (window.currentUser.UserId === attachment.UploaderId) {
                            attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, '<li><a href="javascript://" class="pr-icon-left remove" data-id="' + attachment.Id + '" data-chat-id="' + item.Id + '"><i class="fa fa-trash"></i><span>{deletes}</span></a></li>');
                        } else {
                            attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, "");
                        }

                        attachmentsHtml = attachmentsHtml.replace(/{download}/ig, resources.common.label.download);
                        attachmentsHtml = attachmentsHtml.replace(/{attachmentId}/ig, attachment.Id);
                        attachmentsHtml = attachmentsHtml.replace(/{userType}/ig, self.option.type);
                        attachmentsHtml = attachmentsHtml.replace(/{deletes}/ig, resources.common.label.deletes);
                        attachmentsHtml = attachmentsHtml.replace(/{groupChatId}/ig, self.groupId);
                        attachmentsHtml = attachmentsHtml.replace(/{mediaLeft}/ig, '<div class="media-left"><a href="javascript://">' + getUploadItem(attachment) + '</a></div>');
                        attachmentsHtml = attachmentsHtml.replace(/{mediaRight}/ig, "");
                    });

                    html = html.replace(/{listAttachment}/ig, attachmentsHtml);
                }

                html = html.replace(/{file}/ig, resources.common.label.files);
                html = html.replace(/{all}/ig, resources.common.label.all);
                item.Content = html;
            } else {
                item.Content = smilesFilter(item.Content);
            }
        });
    };

    function execCommentItem(commentItem) {
        var userInfo = window.currentUser;

        commentItem.IsCustomer = commentItem.Type === 1;
        commentItem.Time = moment(commentItem.SentTime).fromNow();
        commentItem.Liked = ko.observable(commentItem.Liked);
        commentItem.NumberOfReplies = ko.observable(commentItem.NumberOfReplies);
        commentItem.Like = ko.observable(commentItem.Like == undefined ? null : commentItem.Like);
        commentItem.LikeText = ko.computed(function () {
            return commentItem.Liked() ? resources.common.label.unLike : resources.common.label.like;
        });
        commentItem.IsShowReply = ko.observable(false);
        commentItem.ListReplies = ko.observableArray([]);
        commentItem.IsSending = ko.observable(false);
        commentItem.IsMine = commentItem.UserId === userInfo.UserId;
        commentItem.Class = commentItem.IsMine ? 'media mine' : 'media';
        commentItem.IsLoadingReply = ko.observable(false);
        commentItem.IsLoadingMoreReply = ko.observable(false);
        commentItem.IsShowLoadmoreReply = ko.computed(function () {
            return commentItem.ListReplies().length < commentItem.NumberOfReplies() && commentItem.NumberOfReplies() > groupChatPageSize && !commentItem.IsLoadingReply();
        });

        if (commentItem.AttachmentCount != null && commentItem.AttachmentCount > 0) {
            var message = $.parseJSON(commentItem.Content);

            // Nếu lớn hơn 5 file hiển thị dạng nhóm.
            var html = '';
            var attachmentsHtml;
            if (commentItem.AttachmentCount > 5) {
                attachmentsHtml = '';
                html = '<div class="wrapper-attachment-compress">\
                        <span class="attachment-count" id="attachment-count-' + commentItem.Id + '">{fileCount}</span>\
                        <span> {file}</span>\
                        <a href="javascript://" class="show-all-attachment" data-target="#attachment-' + commentItem.Id + '">{all}</a>\
                        <ul class="wrapper-list-attachment media-list" id="attachment-' + commentItem.Id + '">{listAttachment}<ul>\
                        </div>';

                _.each(message, function (attachment) {
                    attachmentsHtml += '<li class="media" id="attachment-item-' + attachment.Id + '">\
                            <div class="media-left">\
                                <a href="javascript://">'+ getUploadItem(attachment) + '</a></div>\
                            <div class="media-body">\
                            <h4 class="media-heading attachment-name">' + attachment.AttachmentName + '</h4>\
                            <ul class="wrapper-file-attach-options">\
                                <li><a href="/GroupChat/DownloadAttachment?attachmentId={attachmentId}&userType={userType}&groupChatId={groupChatId}" class="pr-icon-left download" data-id="' + attachment.Id + '" data-chat-id="' + commentItem.Id + '"><i class="fa fa-download"></i><span>{download}</span></a></li>\
                                {removeButton}\
                            </ul>\
                        </div>';

                    if (window.currentUser.UserId === attachment.UploaderId) {
                        attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, '<li><a href="javascript://" class="pr-icon-left remove" data-id="' + attachment.Id + '" data-chat-id="' + commentItem.Id + '"><i class="fa fa-trash"></i><span>{deletes}</span></a></li>');
                    } else {
                        attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, "");
                    }

                    attachmentsHtml = attachmentsHtml.replace(/{deletes}/ig, resources.common.label.deletes);
                    attachmentsHtml = attachmentsHtml.replace(/{download}/ig, resources.common.label.download);
                    attachmentsHtml = attachmentsHtml.replace(/{attachmentId}/ig, attachment.Id);
                    attachmentsHtml = attachmentsHtml.replace(/{userType}/ig, self.option.type);
                    attachmentsHtml = attachmentsHtml.replace(/{groupChatId}/ig, self.groupId);
                });

                html = html.replace(/{fileCount}/ig, commentItem.AttachmentCount);
                html = html.replace(/{listAttachment}/ig, attachmentsHtml);
            } else {
                attachmentsHtml = '';
                html = '<ul class="wrapper-list-attachment media-list">{listAttachment}</ul>';
                _.each(message, function (attachment) {
                    attachmentsHtml += '<li class="media" id="attachment-item-' + attachment.Id + '">\
                            {mediaLeft}\
                            <div class="media-body">\
                            <h4 class="media-heading">' + attachment.AttachmentName + '</h4>\
                            <ul class="wrapper-file-attach-options">\
                                <li><a href="/GroupChat/DownloadAttachment?attachmentId={attachmentId}&userType={userType}&groupChatId={groupChatId}" class="pr-icon-left download" data-id="' + attachment.Id + '" data-chat-id="' + commentItem.Id + '"><i class="fa fa-download"></i><span>{download}</span></a></li>\
                                {removeButton}\
                            </ul>\
                        </div>\
                        {mediaRight}';

                    if (window.currentUser.UserId === attachment.UploaderId) {
                        attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, '<li><a href="javascript://" class="pr-icon-left remove" data-id="' + attachment.Id + '" data-chat-id="' + commentItem.Id + '"><i class="fa fa-trash"></i><span>{deletes}</span></a></li>');
                    } else {
                        attachmentsHtml = attachmentsHtml.replace(/{removeButton}/ig, "");
                    }

                    attachmentsHtml = attachmentsHtml.replace(/{download}/ig, resources.common.label.download);
                    attachmentsHtml = attachmentsHtml.replace(/{attachmentId}/ig, attachment.Id);
                    attachmentsHtml = attachmentsHtml.replace(/{userType}/ig, self.option.type);
                    attachmentsHtml = attachmentsHtml.replace(/{deletes}/ig, resources.common.label.deletes);
                    attachmentsHtml = attachmentsHtml.replace(/{groupChatId}/ig, self.groupId);
                    attachmentsHtml = attachmentsHtml.replace(/{mediaLeft}/ig, '<div class="media-left"><a href="javascript://">' + getUploadItem(attachment) + '</a></div>');
                    attachmentsHtml = attachmentsHtml.replace(/{mediaRight}/ig, "");
                });

                html = html.replace(/{listAttachment}/ig, attachmentsHtml);
            }

            html = html.replace(/{file}/ig, resources.common.label.files);
            html = html.replace(/{all}/ig, resources.common.label.all);
            commentItem.Content = html;
        } else {
            commentItem.Content = smilesFilter(commentItem.Content);
        }

        return commentItem;
    };

    self.objectTitle = ko.observable("");
    self.isLoadingComment = ko.observable(true);
    self.showNoRecord = ko.observable(false);
    self.viewLikedUserError = ko.observable("");
    self.isLoadingUserLiked = ko.observable(false);
    self.isShowNotification = ko.observable(self.option.isShowNotify != undefined && self.option.isShowNotify != null ? self.option.isShowNotify : false);
    self.isShowCommentBox = ko.observable(self.option.isShowCommentBox);
    self.isShowNotificationText = ko.computed(function () {
        return self.isShowNotification() ? resources.common.label.stopReceiveNotification : resources.common.label.receiveNotification;
    });

    self.isShowCommentBox.subscribe(function (newValue) {
        if (newValue) {
            // khởi tạo ô chat.
            setTimeout(function () {
                initEditor();
            }, 200);
        }
    });

    self.listComments = ko.observableArray([]);
    self.listUserLiked = ko.observableArray([]);
    self.listAttachment = ko.observable([]);

    self.like = function (data) {
        data.Liked(!data.Liked());

        if (data.Liked())
            data.Like(data.Like() + 1);
        else
            data.Like(data.Like() - 1);

        $.post("/GroupChat/GroupChatLike", { ContentId: data.Id, groupId: self.groupId, userType: self.option.type, isLike: data.Liked() }, function (result) {
            if (result < 0) {
                data.Liked(!data.Liked());
                return;
            }
        });
    };

    self.changeIsShowNotificationSetting = function () {
        self.isShowNotification(!self.isShowNotification());

        $.post("/GroupChat/StopReceiveNotification", { groupId: self.groupId, userType: self.option.type, isShowNotify: self.isShowNotification() }, function (result) {
            if (result === -1) {
                toast.error("Please login before performing this function.");
                return;
            }

            if (result === -2) {
                toast.error(resources.common.message.notHavePermission);
                return;
            }

            if (result === -3) {
                toast.error("");
                return;
            }
        });
    };

    self.reply = function (data) {
        data.IsShowReply(!data.IsShowReply());

        if (data.IsShowReply() && $("#wrapperModalReply" + data.Id).length === 0) {
            data.IsLoadingReply(true);

            if (typeof permission === "undefined" || permission > 0) {
                var html = '<div id="wrapperModalReply{parentId}" class="wrapper-reply-box pr-mgb-15"><textarea class="form-control" rows="2" id="replyModalBox{parentId}" placeholder="{enterContent}"></textarea>\
                <div class="text-right pr-mgt-10">\
                    <button type="button" class="btn btn-primary send-reply" id="sendModalReply{parentId}" style="display: none;" data-comment-for-id="{parentId}">Gửi</button>\
                </div></div>';

                html = html.replace(/{enterContent}/ig, resources.common.message.enterContent);
                html = html.replace(/{parentId}/ig, data.Id);

                $("#parent-" + data.Id + " .wrapper-sub-list").prepend(html);
                var url = self.option.type === 0 ? "/Upload/UploadAttachment" : "/CustomerUpload/UploadAttachment";

                setTimeout(function () {
                    $("#replyModalBox" + data.Id).henryEditor({
                        fileUpload: {
                            url: url,
                            callback: function (result) {
                                var arrayAttachment = [];
                                _.each(result, function (item) {
                                    arrayAttachment.push(item);
                                });

                                self.listAttachment(arrayAttachment);

                                setTimeout(function () {
                                    self.sendReply(data.Id);
                                    //$("#sendReply" + data.Id).trigger("click");
                                }, 100);
                            },
                            selectImageLabel: resources.common.message.selectFiles,
                            allowFilesType: ["png", "gif", "jpeg", "jpg", "rar", "zip"]
                        },
                        suggestSource: self.option.listUserTag,
                        onKeyPress: function (key) {
                            if (key === 13) {
                                //$("#sendReply" + data.Id).trigger("click");
                                self.sendReply(data.Id);
                                return false;
                            }
                        }
                    });
                    setEditorFocus("#replyModalBox" + data.Id);

                    //$(".main-list").scrollTop($("#replyBox" + data.Id).offset().top);
                }, 200);
            }

            $.get("/GroupChat/GetListGroupChatReplies", { contentId: data.Id, groupId: self.groupId, userType: self.option.type, page: data.ListReplies().length, pageSize: 10 }, function (result) {
                setTimeout(function () {
                    data.IsLoadingReply(false);
                }, 500);

                if (result != undefined) {
                    loopListComments(result, false);

                    data.ListReplies(result);
                }
            });
        } else {
            if (typeof permission != "undefined" && permission > 0) {
                setEditorFocus("#replyModalBox" + data.Id);
            }
        }
    };

    self.loadMoreReply = function (data) {
        if (data.IsLoadingMoreReply()) {
            toastr.warning(resources.common.message.uploadProgressNotDone);
            return;
        }

        data.IsLoadingMoreReply(true);
        $.get("/GroupChat/GetListGroupChatReplies", { contentId: data.Id, groupId: self.groupId, userType: self.option.type, page: data.ListReplies().length, pageSize: 10 }, function (result) {
            setTimeout(function () {
                data.IsLoadingMoreReply(false);
            }, 500);

            if (result != undefined) {
                loopListComments(result);

                data.ListReplies.push.apply(data.ListReplies, result);
            }
        });
    };

    self.showLikeDetail = function (data) {
        self.viewLikedUserError("");
        self.isLoadingUserLiked(true);

        $("#likeDetailModal").modal("show");

        $.get("/GroupChat/GroupChatGetListUserLiked", { contentId: data.Id, groupId: self.groupId, userType: self.option.type }, function (result) {
            self.isLoadingUserLiked(false);

            if (result === -1) {
                self.viewLikedUserError(resources.common.message.notHavePermission);
                return;
            }

            self.listUserLiked(result);
        });
    };

    self.joinGroup = function () {
        groupChat.server.modalJoinGroupChat(self.groupId, self.option.type, 0, 10);
    };

    self.sendMessage = function () {
        var message = $("#commentBoxModal").henryEditor('getText');
        if (($.trim(message) === "" || message === null) && self.listAttachment().length === 0) {
            toastr.warning(resources.common.message.pleaseEnterCommentContent);
            $("#commentBoxModal").henryEditor('setHTML', '');
            return;
        }

        if (self.option.type === -1) {
            toastr.error(resources.common.message.notHavePermission);
            $("#commentBoxModal").henryEditor('setHTML', '');
            return;
        }

        setTimeout(function () {
            $("#commentBoxModal").henryEditor('setHTML', '');
        }, 50);

        //var pagePath = typeof (pageUrl) !== "undefined" ? pageUrl : "";
        groupChat.server.pageModalGroupChatSendMessage(self.groupId, message, self.option.type, self.listAttachment(), self.pageTitle, self.pageUrl);
        self.listAttachment([]);

        self.setMainListScrollTop();
    }

    self.sendReply = function (idComment) {
        //var _id = $(this).attr("data-comment-for-id");
        var editorId = "#replyModalBox" + idComment;
        var replyText = $("#replyModalBox" + idComment).henryEditor("getText");
        if ((replyText === undefined || replyText === "" || $.trim(replyText) === "") && self.listAttachment().length === 0) {
            toastr.warning(resources.common.message.pleaseEnterCommentContent);
            $(editorId).henryEditor('setHTML', '');
            return;
        }

        groupChat.server.groupChatModalReply(idComment, self.groupId, replyText, self.option.type, self.listAttachment());
        $(editorId).henryEditor('setHTML', '');
        self.listAttachment([]);
        setEditorFocus(editorId);
    }

    self.setMainListScrollTop = function () {
        $(".comment-modal-list").scrollTop($(".comment-modal-list").prop("scrollHeight"));
    };

    $(function () {
        self.setMainListScrollTop();
        $(".wrapper-comment-chat .main-list").height($(window).height() - 250);

        $("#groupCommentModal").on("hide", function () {
            self.groupId = "";
        });

        // Phân trang comment
        $(".comment-modal-list").on("scroll", function () {
            var t = $(this);
            var top = t.scrollTop();
            if (top === 0 && self.listComments().length < groupChatTotalRows) {
                // Hiển thị loading
                t.prepend('<li class="loading-more"><div class="wrapper-loading">\
                    <div class="spinner" style="margin: 10px auto; ">\
                        <div class="rect1"></div>\
                        <div class="rect2"></div>\
                        <div class="rect3"></div>\
                        <div class="rect4"></div>\
                        <div class="rect5"></div>\
                    </div>\
                </div></li>');

                // Lấy về nội dung của cuộc hội thoại
                $.post("/GroupChat/GroupChatContentLoadMore", { groupId: self.groupId, type: self.option.type, page: self.listComments().length, pageSize: groupChatPageSize }, function (result) {
                    t.find(".loading-more").remove();

                    if (result === -1) {
                        toastr.error(resources.common.message.notHavePermission);
                        return;
                    }

                    groupChatTotalRows = result.totalRows;
                    loopListComments(result.items);
                    _.each(result.items, function (item) {
                        self.listComments.unshift(item);
                    });
                });
            }
        });

        setTimeout(function () {
            initEditor();
        }, 200);

        // Show all attachment
        $(document).on("click", ".show-all-attachment", function () {
            var target = $(this).attr("data-target");

            if ($(target).is(":visible")) {
                $(target).css("display", "none");
                $(this).html(resources.common.message.showAll);
            } else {
                $(target).css("display", "block");
                $(this).html(resources.common.message.showLess);
            }
        });

        // Clients
        groupChat = $.connection.groupChatHub;
        $.connection.hub.logging = true;
        $.connection.hub.qs = { "type": self.option.type, "groupId": self.groupId };

        // After join to group
        groupChat.client.modalAfterJoinedToGroup = function (contents, totalRows) {
            self.isLoadingComment(false);
            self.showNoRecord(false);

            if (typeof (self.option.isCommentFocus) != "undefined" && self.option.isCommentFocus)
                $("#commentBoxModal").henryEditor("setFocus");

            if (contents != undefined && contents !== "") {
                var contentJson = $.parseJSON(contents);
                contentJson = contentJson.reverse();
                loopListComments(contentJson);
                self.listComments(contentJson);
                groupChatTotalRows = totalRows;

                setTimeout(function () {
                    self.setMainListScrollTop();
                }, 200);
            } else {
                self.showNoRecord(true);
            };
        };

        // After send message
        groupChat.client.modalAfterSendMessage = function (result) {
            if (result === -1) {
                toastr.warning(resources.common.message.notHavePermission);
                return;
            }

            if (result === -2) {
                toastr.warning("Please login again.");
                return;
            }

            var messageItem = $.parseJSON(result);
            var commentItem = execCommentItem(messageItem);
            self.listComments.push(commentItem);

            groupChatTotalRows += 1;
            self.setMainListScrollTop();

            if (self.callback) {
                self.callback(result);
            }
        };

        // After reply message
        groupChat.client.modalAfterReply = function (result) {
            if (result === -1) {
                window.location = "/";
            }

            if (result != undefined) {
                var messageItem = $.parseJSON(result);
                var commentItem = _.find(self.listComments(), function (item) {
                    return item.Id === messageItem.ParentId;
                });

                if (commentItem !== undefined) {
                    // +1 số lượng comment.
                    commentItem.NumberOfReplies(commentItem.NumberOfReplies() === undefined || commentItem.NumberOfReplies() === null ? 1 : commentItem.NumberOfReplies() + 1);
                    if (result != undefined && (commentItem.IsShowReply() || commentItem.ListReplies().length > 0)) {
                        var replyItem = execCommentItem(messageItem);

                        replyItem.Class = ko.computed(function () {
                            return replyItem.IsSending() ? 'sending media review-item review-item-left' : 'media review-item review-item-left';
                        });

                        replyItem.LikeText = ko.computed(function () {
                            return replyItem.Liked() ? resources.common.label.unLike : resources.common.label.like;
                        });

                        commentItem.ListReplies.unshift(replyItem);
                    }
                }

            }
        };

        groupChat.client.afterLike = function (contentId, isLike, chatId) {
            if (chatId != null) {
                var parentChat = _.find(self.listComments(), function (item) {
                    return item.Id === chatId;
                });

                if (parentChat != undefined) {
                    var chatContent = _.find(parentChat.ListReplies(), function (item) {
                        return item.Id === contentId;
                    });

                    if (chatContent != undefined) {
                        chatContent.Like(isLike ? chatContent.Like() + 1 : chatContent.Like() - 1);
                    }
                }
            } else {
                var chatContent = _.find(self.listComments(), function (item) {
                    return item.Id === contentId;
                });

                if (chatContent != undefined) {
                    chatContent.Like(isLike ? chatContent.Like() + 1 : chatContent.Like() - 1);
                }
            }
        };

        groupChat.client.modalAfterRemoveChatContent = function (parentId, chatId) {
            if (parentId != null) {
                // Cập nhập lại số lượng comment.
                var parentChatInfo = _.find(self.listComments(), function (item) {
                    return item.Id === parentId;
                });
                if (parentChatInfo != undefined) {
                    parentChatInfo.NumberOfReplies(parentChatInfo.NumberOfReplies() - 1);
                    parentChatInfo.ListReplies.remove(function (item) {
                        return item.Id === chatId;
                    });
                }
            }

            self.listComments.remove(function (item) {
                return item.Id === chatId;
            });
        }

        // After attachment file removed
        groupChat.client.afterRemoveAttachmentFile = function (attachmentId, chatId, attachmentCount) {
            if (attachmentId === -1) {
                toastr.error(resources.common.message.notHavePermission);
                return;
            }

            $("#attachment-item-" + attachmentId).remove();
            $("#attachment-count-" + chatId).html(attachmentCount + " File");
        };

        // Servers
        $.connection.hub.start().done(function () {
            if (self.groupId !== null) {
                groupChat.server.modalJoinGroupChat(self.groupId, self.option.type, 0, 10);
            }

            // Remove attachment item
            $(document).on("click", "#groupCommentModal .wrapper-file-attach-options .remove", function () {
                var id = $(this).attr("data-id");
                var chatId = $(this).attr("data-chat-id");
                swal({
                    title: "",
                    text: resources.common.message.confirmDelete2,
                    type: "warning", showCancelButton: true,
                    confirmButtonText: resources.common.message.accept, cancelButtonText: resources.common.message.cancel
                }, function (isConfirm) {
                    if (isConfirm) {
                        groupChat.server.modalRemoveAttachmentFile(self.groupId, id, chatId, self.option.type);
                    }
                });
            });
        }).fail(function (error) {
            //window.location = window.location.href;
            console.log("fail to connect to server");
        });
    });
};

