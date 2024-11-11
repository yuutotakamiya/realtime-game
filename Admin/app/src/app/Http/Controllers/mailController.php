<?php

namespace App\Http\Controllers;

use App\Models\Mail;
use App\Models\User;
use App\Models\UserMail;
use Barryvdh\Debugbar\Facades\Debugbar;
use Illuminate\Http\Request;

class mailController extends Controller
{
    //メールマスタの表示
    public function mail_index(Request $request)
    {
        $mails = Mail::selectRaw('mails.id,name,text_message')
            ->join('items', 'items.id', '=', 'mails.item_id')->get();

        return view('mails.mailmaster', ['mails' => $mails]);
    }

    //ユーザー受信メール一覧表示
    public function user_mailList(Request $request)
    {
        $user = User::find($request->id);
        if (!empty($user)) {
            $mails = $user->mails()->paginate(10);
            $mails->appends(['id' => $request->id]);
        }
        return view('mails.mailreception', ['user' => $user]);
    }

    //ユーザーメール送信を表示する
    public function show_send()
    {
        return view('mails.transmission');
    }


    //ユーザーにメールを送信する処理
    public function send(Request $request)
    {
        //バリデーションチェック
        $validated = $request->validate([
            'user_id' => ['required'],
            'mail_id' => ['required']
        ]);
        UserMail::create(['user_id' => $request['user_id'], 'mail_id' => $request['mail_id'], 'condition' => 0]);
        return redirect()->route('mailsuser_mail_list',
            ['user_id' => $request['user_id'], 'mail_id' => $request['mail_id']]);


    }
}
