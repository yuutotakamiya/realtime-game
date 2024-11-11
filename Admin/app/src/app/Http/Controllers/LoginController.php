<?php

namespace App\Http\Controllers;

use App\Models\Account;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use function Laravel\Prompts\password;

class LoginController extends Controller
{
    public function index(Request $request)
    {
        return view('accounts/login');
    }

    //ログインする
    public function dologin(Request $request)
    {
        //バリデーションチェック
        $validated = $request->validate([
            'name' => ['required', 'min:4', 'max:25'],
            'password' => ['required']
        ]);
        $request->session()->put('login', true);
        //条件を指定して取得
        $account = Account::where('name', '=', $request['name'])->get();
        // パスワードが一致していたら次の画面に遷移する
        if ($account->count() == 0) {
            return redirect()->route('login', ['error' => 'invalid']);
        } elseif (Hash::check($request['password'], $account[0]['password'])) {
            return redirect()->route('accounts.gameManagement');
        } else {
            return redirect()->route('login', ['error' => 'invalid']);
        }
    }

    //ログアウトする
    public function dologout(Request $request)
    {
        $request->session()->forget('login');
        $request->session()->flush();
        return redirect('/');
    }
}
