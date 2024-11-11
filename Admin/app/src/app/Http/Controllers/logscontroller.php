<?php

namespace App\Http\Controllers;

use App\Models\follow_logs;
use App\Models\User;
use Illuminate\Http\Request;

class logscontroller extends Controller
{
    //フォローログを表示
    public function show_follow_log(Request $request)
    {
        $user = User::find($request->id);

        if (!empty($user)) {
            $users = $user->logs()->paginate(10);
            $users->appends(['id' => $request->id]);
        }

        return view('Follow.follow_logs', ['users' => $users ?? null]);
    }

    public function show_stage_log()
    {

    }
}
