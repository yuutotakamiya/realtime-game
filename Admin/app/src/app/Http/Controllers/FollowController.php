<?php

namespace App\Http\Controllers;

use App\Models\Follow;
use App\Models\follow_logs;
use App\Models\User;
use Illuminate\Http\Request;

class FollowController extends Controller
{
    //id検索
    public function ShowFollowList(Request $request)
    {
        $user = User::find($request->id);

        //$userが空じゃなかったら
        if (!empty($user)) {
            $users = $user->follows()->paginate(10);
            $users->appends(['id' => $request->id]);

            //相互フォローの表示
            for ($i = 0; $i < count($users); $i++) {
                $follow_user = Follow::where('user_id', '=', $users[$i]['id'])
                    ->where('follow_user_id', '=', $request['id'])->exists();
                $users[$i]['is_mutual'] = $follow_user === true ? 1 : 0;
            }
        }
        return view('Follow.FollowList', ['user' => $user ?? null, 'users' => $users ?? null]);
    }
}
