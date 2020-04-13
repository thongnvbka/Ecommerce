(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], function ($) {
            return factory(window, document, $);
        });
    } else if (typeof exports !== 'undefined') {
        module.exports = factory(window, document, require('jquery'));
    } else {
        return factory(window, document, jQuery);
    }
})(function (window, document, $) {
    'use strict';

    // HoangNH: Emotionicon maps
    var ignoreKeys = [],
        imageExtension = [".jpg", ".png", ".gif", ".jpeg"],
        emoticonLibs = {
            ":)": "&nbsp;<img src='/Images/advance_smiles/smile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;' />&nbsp;",
            ":(": "&nbsp;<img src='/Images/advance_smiles/sadsmile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":D": "&nbsp;<img src='/Images/advance_smiles/bigsmile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":d": "&nbsp;<img src='/Images/advance_smiles/bigsmile_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "8)": "&nbsp;<img src='/Images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "b)": "&nbsp;<img src='/Images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "B)": "&nbsp;<img src='/Images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(cool)": "&nbsp;<img src='/Images/advance_smiles/cool_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(wink)": "&nbsp;<img src='/Images/advance_smiles/wink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ";)": "&nbsp;<img src='/Images/advance_smiles/wink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":o": "&nbsp;<img src='/Images/advance_smiles/surprised_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":O": "&nbsp;<img src='/Images/advance_smiles/surprised_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ";(": "&nbsp;<img src='/Images/advance_smiles/crying_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(sweat)": "&nbsp;<img src='/Images/advance_smiles/sweating_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(:|": "&nbsp;<img src='/Images/advance_smiles/sweating_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":|": "&nbsp;<img src='/Images/advance_smiles/speechless_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":*": "&nbsp;<img src='/Images/advance_smiles/kiss_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(kiss)": "&nbsp;<img src='/Images/advance_smiles/kiss_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":p": "&nbsp;<img src='/Images/advance_smiles/tongueout_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":P": "&nbsp;<img src='/Images/advance_smiles/tongueout_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(yn)": "&nbsp;<img src='/Images/advance_smiles/fingerscrossed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(blush)": "&nbsp;<img src='/Images/advance_smiles/blushing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":$": "&nbsp;<img src='/Images/advance_smiles/blushing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":\">": "&nbsp;<img src='/Images/advance_smiles/blushing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":^)": "&nbsp;<img src='/Images/advance_smiles/wondering_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "|)": "&nbsp;<img src='/Images/advance_smiles/sleepy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(snooze)": "&nbsp;<img src='/Images/advance_smiles/sleepy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "|(": "&nbsp;<img src='/Images/advance_smiles/dull_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(inlove)": "&nbsp;<img src='/Images/advance_smiles/inlove_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(grin)": "&nbsp;<img src='/Images/advance_smiles/evilgrin_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "]:)": "&nbsp;<img src='/Images/advance_smiles/evilgrin_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ">:)": "&nbsp;<img src='/Images/advance_smiles/evilgrin_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(yawn)": "&nbsp;<img src='/Images/advance_smiles/yawning_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(puke)": "&nbsp;<img src='/Images/advance_smiles/puking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":&": "&nbsp;<img src='/Images/advance_smiles/puking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(doh)": "&nbsp;<img src='/Images/advance_smiles/doh_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":@": "&nbsp;<img src='/Images/advance_smiles/angry_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "x(": "&nbsp;<img src='/Images/advance_smiles/angry_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(wasntme)": "&nbsp;<img src='/Images/advance_smiles/itwasntme_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(party)": "&nbsp;<img src='/Images/advance_smiles/party_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(facepalm)": "&nbsp;<img src='/Images/advance_smiles/facepalm_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":S": "&nbsp;<img src='/Images/advance_smiles/worried_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":s": "&nbsp;<img src='/Images/advance_smiles/worried_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(mm)": "&nbsp;<img src='/Images/advance_smiles/mmm_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "8|": "&nbsp;<img src='/Images/advance_smiles/nerd_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "B|": "&nbsp;<img src='/Images/advance_smiles/nerd_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "b|": "&nbsp;<img src='/Images/advance_smiles/nerd_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":x": "&nbsp;<img src='/Images/advance_smiles/lipssealed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":X": "&nbsp;<img src='/Images/advance_smiles/lipssealed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(hi)": "&nbsp;<img src='/Images/advance_smiles/hi_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(wave)": "&nbsp;<img src='/Images/advance_smiles/hi_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(devil)": "&nbsp;<img src='/Images/advance_smiles/devil_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(angle)": "&nbsp;<img src='/Images/advance_smiles/angle_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(envy)": "&nbsp;<img src='/Images/advance_smiles/envy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(wait)": "&nbsp;<img src='/Images/advance_smiles/wait_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(bear)": "&nbsp;<img src='/Images/advance_smiles/hug_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(hug)": "&nbsp;<img src='/Images/advance_smiles/hug_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(makeup)": "&nbsp;<img src='/Images/advance_smiles/makup_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(kate)": "&nbsp;<img src='/Images/advance_smiles/makup_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(moustache)": "&nbsp;<img src='/Images/advance_smiles/moustache_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(giggle)": "&nbsp;<img src='/Images/advance_smiles/giggle_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(chuckle)": "&nbsp;<img src='/Images/advance_smiles/makup_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(clap)": "&nbsp;<img src='/Images/advance_smiles/clap_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":?": "&nbsp;<img src='/Images/advance_smiles/think_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(think)": "&nbsp;<img src='/Images/advance_smiles/think_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(bow)": "&nbsp;<img src='/Images/advance_smiles/bow_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(rofl)": "&nbsp;<img src='/Images/advance_smiles/rofl_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(whew)": "&nbsp;<img src='/Images/advance_smiles/whew_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(highfive)": "&nbsp;<img src='/Images/advance_smiles/highfive_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(happy)": "&nbsp;<img src='/Images/advance_smiles/happy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(smirk)": "&nbsp;<img src='/Images/advance_smiles/smirk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(nod)": "&nbsp;<img src='/Images/advance_smiles/nod_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(shake)": "&nbsp;<img src='/Images/advance_smiles/shake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(emo)": "&nbsp;<img src='/Images/advance_smiles/emo_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(waiting)": "&nbsp;<img src='/Images/advance_smiles/waiting_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(y)": "&nbsp;<img src='/Images/advance_smiles/yes_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(Y)": "&nbsp;<img src='/Images/advance_smiles/yes_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(ok)": "&nbsp;<img src='/Images/advance_smiles/yes_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(n)": "&nbsp;<img src='/Images/advance_smiles/no_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(N)": "&nbsp;<img src='/Images/advance_smiles/no_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(handshake)": "&nbsp;<img src='/Images/advance_smiles/handshake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(h)": "&nbsp;<img src='/Images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(H)": "&nbsp;<img src='/Images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(l)": "&nbsp;<img src='/Images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(L)": "&nbsp;<img src='/Images/advance_smiles/heart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(lalala)": "&nbsp;<img src='/Images/advance_smiles/lalala_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(tmi)": "&nbsp;<img src='/Images/advance_smiles/tmi_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(heidy)": "&nbsp;<img src='/Images/advance_smiles/heidy_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(f)": "&nbsp;<img src='/Images/advance_smiles/flower_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(F)": "&nbsp;<img src='/Images/advance_smiles/flower_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(rain)": "&nbsp;<img src='/Images/advance_smiles/rain_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(sun)": "&nbsp;<img src='/Images/advance_smiles/sunshine_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(music)": "&nbsp;<img src='/Images/advance_smiles/music_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(coffee)": "&nbsp;<img src='/Images/advance_smiles/coffee_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(pizza)": "&nbsp;<img src='/Images/advance_smiles/pizza_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(cash)": "&nbsp;<img src='/Images/advance_smiles/cash_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(flex)": "&nbsp;<img src='/Images/advance_smiles/muscle_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(cake)": "&nbsp;<img src='/Images/advance_smiles/cake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(^)": "&nbsp;<img src='/Images/advance_smiles/cake_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(beer)": "&nbsp;<img src='/Images/advance_smiles/beer_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(d)": "&nbsp;<img src='/Images/advance_smiles/drink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(D)": "&nbsp;<img src='/Images/advance_smiles/drink_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(dance)": "&nbsp;<img src='/Images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "\o/": "&nbsp;<img src='/Images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "\:D/": "&nbsp;<img src='/Images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "\:d/": "&nbsp;<img src='/Images/advance_smiles/dancing_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(ninja)": "&nbsp;<img src='/Images/advance_smiles/ninja_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(*)": "&nbsp;<img src='/Images/advance_smiles/star_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(tumbleweed)": "&nbsp;<img src='/Images/advance_smiles/tumbleweed_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(bandit)": "&nbsp;<img src='/Images/advance_smiles/bandit_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(rock)": "&nbsp;<img src='/Images/advance_smiles/rock_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(banghead)": "&nbsp;<img src='/Images/advance_smiles/banghead_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(punch)": "&nbsp;<img src='/Images/advance_smiles/punch_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(talk)": "&nbsp;<img src='/Images/advance_smiles/talk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(swear)": "&nbsp;<img src='/Images/advance_smiles/swear_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(idea)": "&nbsp;<img src='/Images/advance_smiles/idea_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(talktothehand)": "&nbsp;<img src='/Images/advance_smiles/talktothehand_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(u)": "&nbsp;<img src='/Images/advance_smiles/brokenheart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(U)": "&nbsp;<img src='/Images/advance_smiles/brokenheart_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(fistbump)": "&nbsp;<img src='/Images/advance_smiles/fistbump_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(pray)": "&nbsp;<img src='/Images/advance_smiles/pray_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(poke)": "&nbsp;<img src='/Images/advance_smiles/poke_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(victory)": "&nbsp;<img src='/Images/advance_smiles/victory_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(celebrate)": "&nbsp;<img src='/Images/advance_smiles/celebrate_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(brb)": "&nbsp;<img src='/Images/advance_smiles/brb_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(wfh)": "&nbsp;<img src='/Images/advance_smiles/wfh_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(mbp)": "&nbsp;<img src='/Images/advance_smiles/phone_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(ph)": "&nbsp;<img src='/Images/advance_smiles/phone_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(call)": "&nbsp;<img src='/Images/advance_smiles/call_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(e)": "&nbsp;<img src='/Images/advance_smiles/mail_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(m)": "&nbsp;<img src='/Images/advance_smiles/mail_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(film)": "&nbsp;<img src='/Images/advance_smiles/movie_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(movie)": "&nbsp;<img src='/Images/advance_smiles/movie_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(o)": "&nbsp;<img src='/Images/advance_smiles/time_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(O)": "&nbsp;<img src='/Images/advance_smiles/time_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(time)": "&nbsp;<img src='/Images/advance_smiles/time_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(sheep)": "&nbsp;<img src='/Images/advance_smiles/sheep_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(cat)": "&nbsp;<img src='/Images/advance_smiles/cat_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            ":3": "&nbsp;<img src='/Images/advance_smiles/cat_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(bike)": "&nbsp;<img src='/Images/advance_smiles/bike_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(dog)": "&nbsp;<img src='/Images/advance_smiles/dog_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(soccer)": "&nbsp;<img src='/Images/advance_smiles/soccer_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(monkey)": "&nbsp;<img src='/Images/advance_smiles/monkey_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(bomb)": "&nbsp;<img src='/Images/advance_smiles/bomb_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(man)": "&nbsp;<img src='/Images/advance_smiles/man_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(woman)": "&nbsp;<img src='/Images/advance_smiles/woman_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(drunk)": "&nbsp;<img src='/Images/advance_smiles/drunk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(smoking)": "&nbsp;<img src='/Images/advance_smiles/smoking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(ci)": "&nbsp;<img src='/Images/advance_smiles/smoking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(smoke)": "&nbsp;<img src='/Images/advance_smiles/smoking_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(gottarun)": "&nbsp;<img src='/Images/advance_smiles/gottarun_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(stop)": "&nbsp;<img src='/Images/advance_smiles/stop_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(toivo)": "&nbsp;<img src='/Images/advance_smiles/toivo_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(bug)": "&nbsp;<img src='/Images/advance_smiles/bug_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(poolparty)": "&nbsp;<img src='/Images/advance_smiles/poolparty_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(snail)": "&nbsp;<img src='/Images/advance_smiles/snail_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(goodluck)": "&nbsp;<img src='/Images/advance_smiles/goodluck_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(island)": "&nbsp;<img src='/Images/advance_smiles/island_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(umbrella)": "&nbsp;<img src='/Images/advance_smiles/umbrella_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(rainbow)": "&nbsp;<img src='/Images/advance_smiles/rainbow_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(canyoutalk)": "&nbsp;<img src='/Images/advance_smiles/canyoutalk_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(camera)": "&nbsp;<img src='/Images/advance_smiles/camera_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(plane)": "&nbsp;<img src='/Images/advance_smiles/plane_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(car)": "&nbsp;<img src='/Images/advance_smiles/car_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(computer)": "&nbsp;<img src='/Images/advance_smiles/computer_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(games)": "&nbsp;<img src='/Images/advance_smiles/games_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(gift)": "&nbsp;<img src='/Images/advance_smiles/gift_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(holdon)": "&nbsp;<img src='/Images/advance_smiles/holdon_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(letsmeet)": "&nbsp;<img src='/Images/advance_smiles/letsmeet_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(confidential)": "&nbsp;<img src='/Images/advance_smiles/confidential_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(whatsgoingon)": "&nbsp;<img src='/Images/advance_smiles/whatsgoingon_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(malthe)": "&nbsp;<img src='/Images/advance_smiles/malthe_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(tauri)": "&nbsp;<img src='/Images/advance_smiles/dull_tauri_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(zilmer)": "&nbsp;<img src='/Images/advance_smiles/priidu_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
            "(oliver)": "&nbsp;<img src='/Images/advance_smiles/oliver_80_anim_gif.gif' width='25' height='25' unselectable='on' style='line-height: 1.42857;'/>&nbsp;",
        };


    // http://stackoverflow.com/questions/17242144/javascript-convert-hsb-hsv-color-to-rgb-accurately
    var HSVtoRGB = function (h, s, v) {
        var r, g, b, i, f, p, q, t;
        i = Math.floor(h * 6);
        f = h * 6 - i;
        p = v * (1 - s);
        q = v * (1 - f * s);
        t = v * (1 - (1 - f) * s);
        switch (i % 6) {
            case 0: r = v, g = t, b = p; break;
            case 1: r = q, g = v, b = p; break;
            case 2: r = p, g = v, b = t; break;
            case 3: r = p, g = q, b = v; break;
            case 4: r = t, g = p, b = v; break;
            case 5: r = v, g = p, b = q; break;
        }
        var hr = Math.floor(r * 255).toString(16);
        var hg = Math.floor(g * 255).toString(16);
        var hb = Math.floor(b * 255).toString(16);
        return '#' + (hr.length < 2 ? '0' : '') + hr +
                     (hg.length < 2 ? '0' : '') + hg +
                     (hb.length < 2 ? '0' : '') + hb;
    };

    // Encode htmlentities() - http://stackoverflow.com/questions/5499078/fastest-method-to-escape-html-tags-as-html-entities
    var html_encode = function (string) {
        return string.replace(/[&<>"]/g, function (tag) {
            var charsToReplace = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;'
            };
            return charsToReplace[tag] || tag;
        });
    };

    // Create the Editor
    var create_editor = function ($textarea, classes, placeholder, toolbar_position, toolbar_buttons, toolbar_submit, label_selectImage,
                                  placeholder_url, placeholder_embed, max_imagesize, on_imageupload, force_imageupload, video_from_url,
                                  on_keydown, on_keypress, on_keyup, on_autocomplete, customeOptions) {
        // Content: Insert link
        var wysiwygeditor_insertLink = function (wysiwygeditor, url) {
            if (!url)
                ;
            else if (wysiwygeditor.getSelectedHTML())
                wysiwygeditor.insertLink(url);
            else
                wysiwygeditor.insertHTML('<a href="' + html_encode(url) + '">' + html_encode(url) + '</a>');

            wysiwygeditor.closePopup().collapseSelection();
        };
        var content_insertlink = function (wysiwygeditor, $modify_link) {
            var $button = toolbar_button(toolbar_submit);

            var $inputurl = $('<input type="text" class="form-control" placeholder="Nhập tên đường dẫn" value="' + ($modify_link ? $modify_link.attr('href') : '') + '" />')
                                .keypress(function (event) {
                                    if (event.which != 10 && event.which != 13)
                                        return;

                                    if ($modify_link) {
                                        $modify_link.attr('href', $inputurl.val());
                                        wysiwygeditor.closePopup().collapseSelection();
                                    }
                                    else
                                        wysiwygeditor_insertLink(wysiwygeditor, $inputurl.val());
                                });

            if (placeholder_url)
                $inputurl.attr('placeholder', placeholder_url);

            var $okbutton = $('<span class="input-group-btn"><button class="btn btn-default" type="button"><i class="fa fa-check color-green"></i></button></span>').click(function (event) {
                if ($modify_link) {
                    $modify_link.attr('href', $($inputurl).val());
                    wysiwygeditor.closePopup().collapseSelection();
                }
                else
                    wysiwygeditor_insertLink(wysiwygeditor, $($inputurl).val());

                event.stopPropagation();
                event.preventDefault();
                return false;
            });

            var $content = $('<div/>').addClass('wysiwyg-toolbar-form')
                                      .attr('unselectable', 'on');

            $content.append($('<div class="input-group"/>').append($inputurl).append($okbutton));
            return $content;
        };

        // File into editor
        var getUploadItem = function (item) {
            if (imageExtension.indexOf(item.Extension) > -1) {
                return "<img src='/images/ " + item.AttachmentPath + "_150x150_1.jpg' alt='" + item.AttachmentName + "' />&nbsp;"
            } else {
                return "<div class='attachment-item attachment-" + item.Extension.replace(".", "") + "'><i class='fa fa-" + item.Extension.replace(".", "") + "'></i><span style='text-indent: -9999px; display: block'>1</span></div>&nbsp;";
            }
        };

        var insert_upload_file = function (result) {
            if (result) {
                var attachments = '';
                if (result.length > 1) {
                    attachments += '<ul class="wrapper-list-attachment">';
                    $.each(result, function (index, item) {
                        attachments += '<li>' + getUploadItem(item) + '</li>';
                    });
                    attachments += '</ul>';
                } else {
                    attachments += getUploadItem(result[0]);
                }
                wysiwygeditor.insertHTML(attachments);
            }
        };

        // Content: Insert image
        var content_insertimage = function (wysiwygeditor) {
            // Add image to editor
            var insert_image_wysiwyg = function (url, filename) {
                if (customeOptions.close_popup_after_upload) {
                    wysiwygeditor.closePopup();
                }
                wysiwygeditor.insertHTML("<img src='" + url + "' alt='" + filename + "'/>");
            };

            // Create popup
            var $content = $('<div/>').addClass('wysiwyg-toolbar-form')
                                      .attr('unselectable', 'on');
            // Add image via 'Browse...'
            var $fileuploader = null,
                $fileuploader_input = $('<input type="file" multiple />')
                                        .css({
                                            position: 'absolute',
                                            left: 0,
                                            top: 0,
                                            width: '100%',
                                            height: '100%',
                                            opacity: 0,
                                            cursor: 'pointer'
                                        });

            // HoangNH: Init file upload plugins
            $fileuploader_input.fileupload({
                url: customeOptions.uploadImageUrl ? customeOptions.uploadImageUrl : '/Upload/UploadAttachment',
                sequentialUploads: false,
                singleFileUploads: false,
                dataType: 'json',
                dropZone: null,
                pasteZone: null,
                add: function (e, data) {
                    var allowUpload = true;
                    $.each(data.files, function (index, item) {
                        var name = item.name;
                        var nameArray = name.split(".");
                        var extension = nameArray[nameArray.length - 1];

                        if (imageExtension.indexOf("." + extension) === -1) {
                            allowUpload = false;
                            return;
                        }
                    });

                    if (allowUpload)
                        data.submit();
                    else
                        toastr.error("yuanp tin tải lên không đúng định dạng.");
                },
                done: function (e, data) {
                    if (data.result != undefined) {
                        // Render file                                               
                        if ($.isFunction(customeOptions.after_upload_image))
                            customeOptions.after_upload_image(data.result); // Execute callback function after upload file                                                                                
                        else
                            insert_upload_file(data.result);

                        wysiwygeditor.closePopup();
                    }
                }
            });
            // HoangNH: End upload file

            if (!force_imageupload && window.File && window.FileReader && window.FileList) {
                // File-API
                var loadImageFromFile = function (file) {
                    //// Only process image files
                    //if (!file.type.match('image.*'))
                    //    return;
                    //var reader = new FileReader();
                    //reader.onload = function (event) {
                    //    var dataurl = event.target.result;
                    //    insert_image_wysiwyg(dataurl, file.name);

                    //};
                    //// Read in the image file as a data URL
                    //reader.readAsDataURL(file);
                };
                $fileuploader = $fileuploader_input
                                    .attr('draggable', 'true')
                                    .change(function (event) {
                                        var files = event.target.files; // FileList object
                                        for (var i = 0; i < files.length; ++i)
                                            loadImageFromFile(files[i]);
                                    })
                                    .on('dragover', function (event) {
                                        event.originalEvent.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
                                        event.stopPropagation();
                                        event.preventDefault();
                                        return false;
                                    })
                                    .on('drop', function (event) {
                                        var files = event.originalEvent.dataTransfer.files; // FileList object.
                                        for (var i = 0; i < files.length; ++i)
                                            loadImageFromFile(files[i]);
                                        event.stopPropagation();
                                        event.preventDefault();
                                        return false;
                                    });
            }
            else if (on_imageupload) {
                // Upload image to a server
                var $input = $fileuploader_input
                                    .change(function (event) {
                                        on_imageupload.call(this, insert_image_wysiwyg);
                                    });
                $fileuploader = $('<form/>').append($input);
            }
            if ($fileuploader)
                $('<div/>').addClass('wysiwyg-browse')
                           .html(customeOptions.selectImagePlaceholder ? customeOptions.selectImagePlaceholder : "")
                           .append($fileuploader)
                           .appendTo($content);

            // Add image via 'URL'
            var $inputurl = $('<input type="text" class="form-control" placeholder="Nhập đường dẫn file">').keypress(function (event) {
                if (event.which == 10 || event.which == 13) {
                    insert_image_wysiwyg($inputurl.val());
                }

            });
            var $okButton = $('<span class="input-group-btn"><button class="btn btn-default" type="button"><i class="fa fa-check color-green"></i></button></span>').click(function (event) {
                wysiwygeditor.closePopup();
                insert_image_wysiwyg($inputurl.val());
                event.stopPropagation();
                event.preventDefault();
                return false;
            });
            $content.append($('<div class="input-group"/>').append($inputurl).append($okButton));
            return $content;
        };

        // HoangNH: Content upload file
        var content_upload_file = function (wysiwygeditor) {
            // Create popup
            var $content = $('<div/>').addClass('wysiwyg-toolbar-form')
                                      .attr('unselectable', 'on');
            // Add image via 'Browse...'
            var $fileuploader = null,
                $fileuploader_input = $('<input type="file" multiple />')
                                        .css({
                                            position: 'absolute',
                                            left: 0,
                                            top: 0,
                                            width: '100%',
                                            height: '100%',
                                            opacity: 0,
                                            cursor: 'pointer'
                                        });

            // HoangNH: Init file upload plugins
            var maxFileLength = 5120000;
            $fileuploader_input.fileupload({
                url: customeOptions.fileupload_url ? customeOptions.fileupload_url : '/Upload/UploadAttachment',
                sequentialUploads: false,
                singleFileUploads: false,
                dataType: 'json',
                dropZone: null,
                pasteZone: null,
                add: function (e, data) {
                    var allowFilesType = customeOptions.upload_options.allowFilesType;
                    var ext = data.files[0].name.replace(/^.*\./, '');
                    ext = $.trim(ext.replace('.', '')).toLowerCase();

                    if (typeof (allowFilesType) === "undefined") {
                        allowFilesType = ["png", "gif", "jpeg", "jpg", "rar", "zip"];
                    }
                                        
                    if (allowFilesType.indexOf(ext) === -1) {
                        toastr.error(resources.common.message.invalidFileTypes.format(allowFilesType.join()));
                        return;
                    }                        

                    if (data.files[0].size > maxFileLength) {
                        toastr.error(resources.common.message.maxFileLengthUploadFormat.format("5Mb"));
                        return;
                    }

                    data.submit();
                },
                done: function (e, data) {
                    if (data.result != undefined) {
                        // Render file                                               
                        if ($.isFunction(customeOptions.after_upload_file))
                            customeOptions.after_upload_file(data.result); // Execute callback function after upload file                                                                                
                        else
                            insert_upload_file(data.result);

                        if (customeOptions.close_popup_after_upload) {
                            wysiwygeditor.closePopup();
                        }
                    }
                }
            });
            // HoangNH: End upload file

            if (!force_imageupload && window.File && window.FileReader && window.FileList) {
                $fileuploader = $fileuploader_input
                                    .attr('draggable', 'true')
                                    .change(function (event) {
                                        var files = event.target.files; // FileList object
                                        //for (var i = 0; i < files.length; ++i)
                                        //loadImageFromFile(files[i]);
                                    })
                                    .on('dragover', function (event) {
                                        event.originalEvent.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
                                        event.stopPropagation();
                                        event.preventDefault();
                                        return false;
                                    })
                                    .on('drop', function (event) {
                                        var files = event.originalEvent.dataTransfer.files; // FileList object.
                                        //for (var i = 0; i < files.length; ++i)
                                        //    loadImageFromFile(files[i]);
                                        event.stopPropagation();
                                        event.preventDefault();
                                        return false;
                                    });
            }
            else if (customeOptions.on_fileupload) {
                // Upload image to a server                
                var $input = $fileuploader_input
                                    .change(function (event) {
                                        customeOptions.on_fileupload.call(this, insert_upload_file);
                                    });
                $fileuploader = $('<form/>').append($input);
            }

            if ($fileuploader)
                $('<div/>').addClass('wysiwyg-browse')
                           .html(customeOptions.select_file_text)
                           .append($fileuploader)
                           .appendTo($content);

            // Add image via 'URL'
            // Bắt đường dẫn nhập vào. Làm sau
            //var $inputurl = $('<input type="text" class="form-control" placeholder="Nhập đường dẫn file">').addClass('wysiwyg-input-upload-file')
            //                    .keypress(function (event) {
            //                        if (event.which == 10 || event.which == 13)
            //                            insert_image_wysiwyg($inputurl.val());
            //                    });

            //if (placeholder_url)
            //    $inputurl.attr('placeholder', 'Nhập đường đẫn file'); // HoangNH: Hiện tại đang fix. Sau này Edit lại để có thể đa ngôn ngữ

            //var $okaybutton = $('<span class="input-group-btn"><button class="btn btn-default" type="button"><i class="fa fa-check color-green"></i></button></span>').click(function (event) {
            //    insert_image_wysiwyg($inputurl.val());
            //    event.stopPropagation();
            //    event.preventDefault();
            //    return false;
            //});
            //$content.append($('<div class="input-group"/>').append($inputurl).append($okaybutton));
            return $content;
        };

        // Content: Insert video
        var content_insertvideo = function (wysiwygeditor) {
            // Add video to editor
            var insert_video_wysiwyg = function (url, html) {
                url = $.trim(url || '');
                html = $.trim(html || '');
                var website_url = false;
                if (url.length && !html.length)
                    website_url = url;
                else if (html.indexOf('<') == -1 && html.indexOf('>') == -1 &&
                         html.match(/^(?:https?:\/)?\/?(?:[^:\/\s]+)(?:(?:\/\w+)*\/)(?:[\w\-\.]+[^#?\s]+)(?:.*)?(?:#[\w\-]+)?$/))
                    website_url = html;
                if (website_url && video_from_url)
                    html = video_from_url(website_url) || '';
                if (!html.length && website_url)
                    html = '<video src="' + html_encode(website_url) + '" />';
                wysiwygeditor.insertHTML(html).closePopup().collapseSelection();
            };
            // Create popup
            var $content = $('<div/>').addClass('wysiwyg-toolbar-form')
                                      .attr('unselectable', 'on');
            // Add video via '<embed/>'
            var $textareaembed = $('<textarea placeholder="Nhập mã nhúng">').addClass('wysiwyg-input wysiwyg-inputtextarea');
            if (placeholder_embed)
                $textareaembed.attr('placeholder', placeholder_embed);
            $('<div/>').addClass('wysiwyg-embedcode')
                       .append($textareaembed)
                       .appendTo($content);
            // Add video via 'URL'
            var $button = toolbar_button(toolbar_submit);
            var $inputurl = $('<input type="text" class="form-control" placeholder="Nhập đường dẫn video (youtube hoặc vimeo)">')
                                .keypress(function (event) {
                                    if (event.which == 10 || event.which == 13)
                                        insert_video_wysiwyg($inputurl.val());
                                });

            var $okaybutton = $('<span class="input-group-btn"><button class="btn btn-default" type="button"><i class="fa fa-check"></button></span>').click(function (event) {
                insert_video_wysiwyg($inputurl.val(), $textareaembed.val());
                event.stopPropagation();
                event.preventDefault();
                return false;
            });
            $content.append($('<div class="input-group"/>').append($inputurl).append($okaybutton));
            return $content;
        };

        // Content: Color palette
        var content_colorpalette = function (wysiwygeditor, forecolor) {
            var $content = $('<table/>')
                            .attr('cellpadding', '0')
                            .attr('cellspacing', '0')
                            .attr('unselectable', 'on');
            for (var row = 1; row < 15; ++row) // should be '16' - but last line looks so dark
            {
                var $rows = $('<tr/>');
                for (var col = 0; col < 25; ++col) // last column is grayscale
                {
                    var color;
                    if (col == 24) {
                        var gray = Math.floor(255 / 13 * (14 - row)).toString(16);
                        var hexg = (gray.length < 2 ? '0' : '') + gray;
                        color = '#' + hexg + hexg + hexg;
                    }
                    else {
                        var hue = col / 24;
                        var saturation = row <= 8 ? row / 8 : 1;
                        var value = row > 8 ? (16 - row) / 8 : 1;
                        color = HSVtoRGB(hue, saturation, value);
                    }
                    $('<td/>').addClass('wysiwyg-toolbar-color')
                              .attr('title', color)
                              .attr('unselectable', 'on')
                              .css({ backgroundColor: color })
                              .click(function () {
                                  var color = this.title;
                                  if (forecolor)
                                      wysiwygeditor.forecolor(color).closePopup().collapseSelection();
                                  else
                                      wysiwygeditor.highlight(color).closePopup().collapseSelection();
                                  return false;
                              })
                              .appendTo($rows);
                }
                $content.append($rows);
            }
            return $content;
        };

        // Handlers
        var get_toolbar_handler = function (name, popup_callback) {
            switch (name) {
                case 'insertimage':
                    if (!popup_callback)
                        return null;
                    return function (target) {
                        popup_callback(content_insertimage(wysiwygeditor), target);
                    };
                case 'insertvideo':
                    if (!popup_callback)
                        return null;
                    return function (target) {
                        popup_callback(content_insertvideo(wysiwygeditor), target);
                    };
                case 'insertlink':
                    if (!popup_callback)
                        return null;
                    return function (target) {
                        popup_callback(content_insertlink(wysiwygeditor), target);
                    };
                case 'bold':
                    return function () {
                        wysiwygeditor.bold(); // .closePopup().collapseSelection()
                    };
                case 'italic':
                    return function () {
                        wysiwygeditor.italic(); // .closePopup().collapseSelection()
                    };
                case 'underline':
                    return function () {
                        wysiwygeditor.underline(); // .closePopup().collapseSelection()
                    };
                case 'strikethrough':
                    return function () {
                        wysiwygeditor.strikethrough(); // .closePopup().collapseSelection()
                    };
                case 'forecolor':
                    if (!popup_callback)
                        return null;
                    return function (target) {
                        popup_callback(content_colorpalette(wysiwygeditor, true), target);
                    };
                case 'highlight':
                    if (!popup_callback)
                        return null;
                    return function (target) {
                        popup_callback(content_colorpalette(wysiwygeditor, false), target);
                    };
                case 'alignleft':
                    return function () {
                        wysiwygeditor.align('left'); // .closePopup().collapseSelection()
                    };
                case 'aligncenter':
                    return function () {
                        wysiwygeditor.align('center'); // .closePopup().collapseSelection()
                    };
                case 'alignright':
                    return function () {
                        wysiwygeditor.align('right'); // .closePopup().collapseSelection()
                    };
                case 'alignjustify':
                    return function () {
                        wysiwygeditor.align('justify'); // .closePopup().collapseSelection()
                    };
                case 'subscript':
                    return function () {
                        wysiwygeditor.subscript(); // .closePopup().collapseSelection()
                    };
                case 'superscript':
                    return function () {
                        wysiwygeditor.superscript(); // .closePopup().collapseSelection()
                    };
                case 'indent':
                    return function () {
                        wysiwygeditor.indent(); // .closePopup().collapseSelection()
                    };
                case 'outdent':
                    return function () {
                        wysiwygeditor.indent(true); // .closePopup().collapseSelection()
                    };
                case 'orderedList':
                    return function () {
                        wysiwygeditor.insertList(true); // .closePopup().collapseSelection()
                    };
                case 'unorderedList':
                    return function () {
                        wysiwygeditor.insertList(); // .closePopup().collapseSelection()
                    };
                case 'removeformat':
                    return function () {
                        wysiwygeditor.removeFormat().closePopup().collapseSelection();
                    };
                case 'uploadFile':
                    if (!popup_callback)
                        return null;
                    return function (target) {
                        popup_callback(content_upload_file(wysiwygeditor), target);
                    };
            }
            return null;
        }

        // Create the toolbar
        var toolbar_button = function (button) {
            var returnButton = $('<a/>').addClass('wysiwyg-toolbar-icon')
                            .attr('href', 'javascript://')
                            .attr('title', typeof (button) === "undefined" ? "" : button.title)
                            .attr('unselectable', 'on')
                            .append(typeof (button) === "undefined" ? "" : button.image);

            if (typeof(button) !== "undefined" && button.classes != undefined) {
                returnButton.addClass(button.classes);
            }

            return returnButton;
        };
        var add_buttons_to_toolbar = function ($toolbar, selection, popup_open_callback, popup_position_callback) {
            $.each(toolbar_buttons, function (key, value) {
                if (!value)
                    return;
                // Skip buttons on the toolbar
                if (selection === false && 'showstatic' in value && !value.showstatic)
                    return;
                // Skip buttons on selection
                if (selection === true && 'showselection' in value && !value.showselection)
                    return;

                // HoangNH.
                var $popup;

                // Click handler                
                var toolbar_handler;
                if ('click' in value)
                    toolbar_handler = function (target) {
                        value.click($(target));
                    };
                else if ('popup' in value) {
                    toolbar_handler = function (target) {
                        $popup = popup_open_callback();
                        var overwrite_offset = value.popup($popup, $(target));
                        popup_position_callback($popup, target, overwrite_offset);
                    };
                }
                else
                    toolbar_handler = get_toolbar_handler(key, function ($content, target) {
                        $popup = popup_open_callback();
                        $popup.append($content);
                        popup_position_callback($popup, target, null);
                        $popup.find('input[type=text]:first').focus();
                    });

                // Create the toolbar button
                var $button;
                if (toolbar_handler)
                    $button = toolbar_button(value).click(function (event) {
                        toolbar_handler(event.currentTarget);
                        // Give the focus back to the editor. Technically not necessary
                        if (get_toolbar_handler(key)) // only if not a popup-handler
                            wysiwygeditor.getElement().focus();

                        event.stopPropagation();
                        event.preventDefault();
                        return false;
                    });
                else if (value.html)
                    $button = $(value.html);

                // Add css for button                
                if ($button)
                    $toolbar.append($button);
            });
        };
        var popup_position = function ($popup, $container, left, top, $appentTo, type)  // left+top relative to $container
        {
            // Test parents
            var container_node = $container.get(0),
                offsetparent = container_node.offsetParent,
                offsetparent_offset = { left: 0, top: 0 },  //$.offset() does not work with Safari 3 and 'position:fixed'
                offsetparent_fixed = false,
                offsetparent_overflow = false,
                node = offsetparent;

            while (node) {
                offsetparent_offset.left += node.offsetLeft;
                offsetparent_offset.top += node.offsetTop;
                var $node = $(node);
                if ($node.css("position") == "fixed")
                    offsetparent_fixed = true;
                if ($node.css("overflow") != "visible")
                    offsetparent_overflow = true;
                node = node.offsetParent;
            }

            // Move $popup as high as possible in the DOM tree: offsetParent of $container
            var $offsetparent;           
            // HoangNH: Override apprend popup to button have popup 
            // Append Arrow
            if (customeOptions.isShowArrow) {
                $popup.prepend("<div class='wrapper-arrow' style='position: relative'><div class='arrow arrow-up'></div></div>");
                // Remove Arrow
                $popup.find(".arrow").removeClass("arrow-up arrow-left arrow-right arrow-down");
            }

            if (type === "hover") {
                $popup.find(".arrow").css({ "left": 0, "right": 0, "margin-left": "auto", "margin-right": "auto" }).addClass("arrow-up");
                left += container_node.offsetLeft; // $container.position() does not work with Safari 3
                top += container_node.offsetTop;
                $offsetparent = $(offsetparent || document.body);
            } else {
                left = $appentTo.offset().left;
                top = $appentTo.offset().top;
                $offsetparent = $(document.body);
            }

            var _popupWidth = $($popup).outerWidth();
            var _popupHeight = $($popup).outerHeight();
            var _popupTop = $($popup).offset().top;
            var _popupLeft = $($popup).offset().left;
            var _targetH = $appentTo.outerHeight();
            var _targetW = $appentTo.outerWidth();

            $offsetparent.append($popup);

            // Trim to viewport
            var _docWidth = $(document).width();
            var _docHeight = $(document).height();

            if (_docWidth - left < _popupWidth + left) {
                left = left + _targetW - _popupWidth;
            }            
            $popup.find(".arrow").addClass("arrow-up").css({ "left": 10, "top": -14 });

            if (!type) {
                top = top + _targetH + 10;
            }

            if (left < 0)
                left = 0;

            // Set offset
            $popup.css({
                left: parseInt(left) + "px",
                top: parseInt(top) + "px"
            });
        };

        function emoticonFilter(key, shiftKey, altKey, ctrlKey, metaKey) {
            if (key === 32) {
                var _oldHtml = wysiwygeditor.getHTML();
                var _newHtml = _oldHtml.replace(/(\([a-zA-Z0-9]*\)|[a-zA-Z0-9\:\|\)\(\;\(\]\>\<\[\*]*)*/gi, function (a, b) {
                    return emoticonLibs[b] || a;
                });
                if (_oldHtml != _newHtml) {
                    wysiwygeditor.setHTML("");
                    wysiwygeditor.insertHTML(_newHtml);
                }
            }
        };

        // Transform the textarea to contenteditable
        var hotkeys = {},
            autocomplete = null;

        var create_wysiwyg = function ($textarea, $container, placeholder) {
            var handle_autocomplete = function (keypress, key, character, shiftKey, altKey, ctrlKey, metaKey) {                
                if (!on_autocomplete)
                    return;

                var typed = autocomplete || '';
                switch (key) {
                    case 8: // backspace
                        typed = typed.substring(0, typed.length - 1);
                        // fall through
                    case 13: // enter
                    case 27: // escape
                    case 33: // pageUp
                    case 34: // pageDown
                    case 35: // end
                    case 36: // home
                    case 37: // left
                    case 38: // up
                    case 39: // right
                    case 40: // down
                        if (keypress)
                            return;
                        character = false;
                        break;
                    default:
                        if (!keypress)
                            return;
                        typed += character;
                        break;
                }
                var rc = on_autocomplete(typed, key, character, shiftKey, altKey, ctrlKey, metaKey);
                if (typeof (rc) == 'object' && rc.length) {
                    // Show autocomplete
                    var $popup = $(wysiwygeditor.openPopup());
                    $popup.hide().addClass('wysiwyg-popup wysiwyg-popuphover') // show later
                          .empty().append(rc);
                    autocomplete = typed;
                }
                else {
                    // Hide autocomplete                    
                    wysiwygeditor.closePopup();
                    autocomplete = null;
                    return rc; // swallow key if 'false'
                }
            };

            // Options to wysiwyg.js
            var option = {
                element: $textarea.get(0),
                onKeyDown: function (key, character, shiftKey, altKey, ctrlKey, metaKey) {
                    // Ask master
                    if (on_keydown && on_keydown(key, character, shiftKey, altKey, ctrlKey, metaKey) === false)
                        return false; // swallow key
                    // Exec hotkey (onkeydown because e.g. CTRL+B would oben the bookmarks)
                    if (character && !shiftKey && !altKey && ctrlKey && !metaKey) {
                        var hotkey = character.toLowerCase();
                        if (!hotkeys[hotkey])
                            return;
                        hotkeys[hotkey]();
                        return false; // prevent default
                    }

                    // Handle autocomplete
                    return handle_autocomplete(false, key, character, shiftKey, altKey, ctrlKey, metaKey);
                },
                onKeyPress: function (key, character, shiftKey, altKey, ctrlKey, metaKey) {
                    // Ask master
                    if (on_keypress && on_keypress(key, character, shiftKey, altKey, ctrlKey, metaKey) === false)
                        return false; // swallow key
                    // Handle autocomplete                      
                    return handle_autocomplete(true, key, character, shiftKey, altKey, ctrlKey, metaKey);
                },
                onKeyUp: function (key, character, shiftKey, altKey, ctrlKey, metaKey) {
                    // HoangNH: Handle filter emotionicon
                    if (customeOptions.enableEmoticonFilter) {
                        // filter emotionicon chat
                        emoticonFilter(key, shiftKey, altKey, ctrlKey, metaKey);
                    }

                    // Ask master
                    if (on_keyup && on_keyup(key, character, shiftKey, altKey, ctrlKey, metaKey) === false)
                        return false; // swallow key
                },
                onSelection: function (collapsed, rect, nodes, rightclick) {
                    var show_popup = true,
                        $special_popup = null;
                    // Click on a link opens the link-popup
                    if (collapsed)
                        $.each(nodes, function (index, node) {
                            if ($(node).parents('a').length != 0) { // only clicks on text-nodes
                                $special_popup = content_insertlink(wysiwygeditor, $(node).parents('a:first'))
                                return false; // break
                            }
                        });
                    // Read-Only?
                    if (wysiwygeditor.readOnly())
                        show_popup = false;
                        // Fix type error - https://github.com/wysiwygjs/wysiwyg.js/issues/4
                    else if (!rect)
                        show_popup = false;
                        // Force a special popup?
                    else if ($special_popup)
                            ;
                        // A right-click always opens the popup
                    else if (rightclick)
                            ;
                        // Autocomplete popup?
                    else if (autocomplete)
                            ;
                        // No selection-popup wanted?
                    else if (toolbar_position != 'selection' && toolbar_position != 'top-selection' && toolbar_position != 'bottom-selection')
                        show_popup = false;
                        // Selected popup wanted, but nothing selected (=selection collapsed)
                    else if (collapsed)
                        show_popup = false;
                        // Only one image? Better: Display a special image-popup
                    else if (nodes.length == 1 && nodes[0].nodeName == 'IMG') // nodes is not a sparse array
                        show_popup = false;
                    if (!show_popup) {
                        wysiwygeditor.closePopup();
                        return;
                    }
                    var keyNumOnSelection = 0;
                    $.each(toolbar_buttons, function (key, value) {                        
                        if (value != null && value.showselection !== false) {
                            keyNumOnSelection += 1;
                        }
                    });
                    if (customeOptions.isShowMenuOnSelect && keyNumOnSelection > 0) {
                        // Popup position
                        var $popup;
                        var apply_popup_position = function () {
                            var popup_width = $popup.outerWidth();
                            // Point is the center of the selection - relative to $container not the element
                            var container_offset = $container.offset(),
                                editor_offset = $(wysiwygeditor.getElement()).offset();
                            var left = rect.left + parseInt(rect.width / 2) - parseInt(popup_width / 2) + editor_offset.left - container_offset.left;
                            var top = rect.top + rect.height + editor_offset.top - container_offset.top;

                            popup_position($popup, $container, left, top, $(wysiwygeditor.getElement()), 'hover');
                        };
                        // Open popup
                        $popup = $(wysiwygeditor.openPopup());
                        // if wrong popup -> close and open a new one
                        if (!$popup.hasClass('wysiwyg-popuphover') || (!$popup.data('special')) != (!$special_popup))
                            $popup = $(wysiwygeditor.closePopup().openPopup());
                        if (autocomplete)
                            $popup.show();
                        else if (!$popup.hasClass('wysiwyg-popup')) {
                            // add classes + buttons
                            $popup.addClass('wysiwyg-popup wysiwyg-popuphover');
                            if ($special_popup)
                                $popup.empty().append($special_popup).data('special', true);
                            else {
                                add_buttons_to_toolbar($popup, true,
                                    function () {
                                        return $popup.empty();
                                    },
                                    apply_popup_position);
                            }
                        }
                        // Apply position                    
                        apply_popup_position();
                    }
                },
                onClosepopup: function () {
                    autocomplete = null;
                },
                hijackContextmenu: (toolbar_position == 'selection'),
                readOnly: !!$textarea.attr('readonly')
            };
            if (placeholder) {
                var $placeholder = $('<div/>').addClass('wysiwyg-placeholder')
                                              .html(placeholder)
                                              .hide();
                $container.prepend($placeholder);
                option.onPlaceholder = function (visible) {
                    if (visible)
                        $placeholder.show();
                    else
                        $placeholder.hide();
                };
            }

            var wysiwygeditor = wysiwyg(option);
            return wysiwygeditor;
        }


        // Create a container
        var $container = $('<div/>').addClass('wysiwyg-container');
        if (classes)
            $container.addClass(classes);
        $textarea.wrap($container);
        $container = $textarea.parent('.wysiwyg-container');

        // Create the editor-wrapper if placeholder
        var $wrapper = false;
        if (placeholder) {
            $wrapper = $('<div/>').addClass('wysiwyg-wrapper')
                                  .click(function () {     // Clicking the placeholder focus editor - fixes IE6-IE8
                                      wysiwygeditor.getElement().focus();
                                  });
            $textarea.wrap($wrapper);
            $wrapper = $textarea.parent('.wysiwyg-wrapper');
        }

        // Create the WYSIWYG Editor
        var wysiwygeditor = create_wysiwyg($textarea, placeholder ? $wrapper : $container, placeholder);
        if (wysiwygeditor.legacy) {
            var $textarea = $(wysiwygeditor.getElement());
            $textarea.addClass('wysiwyg-textarea');
            if ($textarea.is(':visible')) // inside the DOM
                $textarea.width($container.width() - ($textarea.outerWidth() - $textarea.width()));
        }
        else {
            $(wysiwygeditor.getElement()).addClass('wysiwyg-editor');

            if (customeOptions.height !== undefined && customeOptions.height > 0) {
                $(wysiwygeditor.getElement()).css("height", customeOptions.height);
            }
        }


        // Hotkey+Commands-List
        var commands = {};
        $.each(toolbar_buttons, function (key, value) {
            if (!value || !value.hotkey)
                return;
            var toolbar_handler = get_toolbar_handler(key);
            if (!toolbar_handler)
                return;
            hotkeys[value.hotkey.toLowerCase()] = toolbar_handler;
            commands[key] = toolbar_handler;
        });

        // Toolbar on top or bottom
        if (!$.isEmptyObject(toolbar_buttons) && toolbar_position != 'selection') {
            var toolbar_top = toolbar_position == 'top' || toolbar_position == 'top-selection';
            var $toolbar = $('<div/>').addClass('wysiwyg-toolbar').addClass(toolbar_top ? 'wysiwyg-toolbar-top' : 'wysiwyg-toolbar-bottom');
            add_buttons_to_toolbar($toolbar, false,
                function () {
                    // Open a popup from the toolbar
                    var $popup = $(wysiwygeditor.openPopup());
                    // if wrong popup -> create a new one
                    if ($popup.hasClass('wysiwyg-popup') && $popup.hasClass('wysiwyg-popuphover'))
                        $popup = $(wysiwygeditor.closePopup().openPopup());
                    if (!$popup.hasClass('wysiwyg-popup'))
                        // add classes + content
                        $popup.addClass('wysiwyg-popup');
                    return $popup;
                },
                function ($popup, target, overwrite_offset) {
                    //// Popup position
                    var $button = $(target);
                    //var popup_width = $popup.outerWidth();
                    //// Point is the top/bottom-center of the button
                    //var left = $button.offset().left - $container.offset().left + parseInt($button.width() / 2) - parseInt(popup_width / 2);
                    //var top = $button.offset().top - $container.offset().top;
                    //if (toolbar_top)
                    //    top += $button.outerHeight();
                    //else
                    //    top -= $popup.outerHeight();
                    //if (overwrite_offset) {
                    //    left = overwrite_offset.left;
                    //    top = overwrite_offset.top;
                    //}                    
                    popup_position($popup, $container, 0, 0, $button);
                });
            if (toolbar_top)
                $container.prepend($toolbar);
            else
                $container.append($toolbar);
        }

        // Export userdata
        return {
            wysiwygeditor: wysiwygeditor,
            $container: $container
        };
    };

    // jQuery Interface
    $.fn.wysiwyg = function (option, param) {
        if (!option || typeof (option) === 'object') {
            option = $.extend({}, option);
            return this.each(function () {
                var $that = $(this);

                // Already an editor
                if ($that.data('wysiwyg'))
                    return;

                // Two modes: toolbar on top and on bottom                
                var classes = option.classes,
                    placeholder = option.placeholder || $that.attr('placeholder'),
                    toolbar_position = (option.toolbar && (option.toolbar == 'top' || option.toolbar == 'top-selection' || option.toolbar == 'bottom' || option.toolbar == 'bottom-selection' || option.toolbar == 'selection')) ? option.toolbar : 'top-selection',
                    toolbar_buttons = option.buttons || {},
                    toolbar_submit = option.submit,
                    label_selectImage = option.selectImage,
                    placeholder_url = option.placeholderUrl || null,
                    placeholder_embed = option.placeholderEmbed || null,
                    max_imagesize = option.maxImageSize || null,
                    on_imageupload = option.onImageUpload || null,
                    force_imageupload = option.forceImageUpload && on_imageupload,
                    //on_fileupload = option.onFileUpload || null, // HoangNH: Added for upload file
                    //fileupload_url = option.fileupload_url, // HoangNH: Added fileupload Url for upload file                    
                    //close_popup_after_upload = option.close_popup_after_upload, // HoangNH: Close popup after upload file
                    video_from_url = option.videoFromUrl || null,
                    on_keydown = option.onKeyDown || null,
                    on_keypress = option.onKeyPress || null,
                    on_keyup = option.onKeyUp || null,
                    on_autocomplete = option.onAutocomplete || null,
                    customeOptions = {
                        select_file_text: option.select_file_text,
                        on_fileupload: option.onFileUpload || null, // HoangNH: Added for upload file
                        fileupload_url: option.fileupload_url, // HoangNH: Added fileupload Url for upload file                    
                        close_popup_after_upload: option.close_popup_after_upload, // HoangNH: Close popup after upload file
                        after_upload_file: option.afterUploadFile, // HoangNH: After upload file callback function,
                        isShowMenuOnSelect: option.isShowMenuOnSelect,
                        enableEmoticonFilter: option.enableEmoticonFilter,
                        height: option.height,
                        isShowArrow: option.isShowArrow,
                        uploadImageUrl: option.uploadImageUrl,
                        after_upload_image: option.afterUploadImage,
                        selectImagePlaceholder: option.selectImagePlaceholder,
                        upload_options: option.upload_options
                    };

                // Create the WYSIWYG Editor
                var data = create_editor($that, classes, placeholder, toolbar_position, toolbar_buttons, toolbar_submit, label_selectImage,
                                          placeholder_url, placeholder_embed, max_imagesize, on_imageupload, force_imageupload, video_from_url,
                                          on_keydown, on_keypress, on_keyup, on_autocomplete, customeOptions);
                $that.data('wysiwyg', data);
            });
        }
        else if (this.length == 1) {
            var data = this.data('wysiwyg');
            if (!data)
                return this;
            if (option == 'container')
                return data.$container;
            if (option == 'shell')
                return data.wysiwygeditor;
        }
        return this;
    };
});
