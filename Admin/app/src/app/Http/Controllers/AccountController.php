<?php

namespace App\Http\Controllers;

use App\Models\Account;
use Barryvdh\Debugbar\Facades\Debugbar;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;

class AccountController extends Controller
{
    //アカウント一覧を表示する
    public function index(Request $request)
    {
        //テーブルの全てのレコードを取得
        //$accounts = Account::all();
        $accounts = Account::Paginate(5);

        return view('accounts/index', ['accounts' => $accounts]);

        //return view('accounts/index',['title'=>$title]);

        //セッションに指定のキーで値を保存
        //$request->session()->put('account_key',1);

        //セッションから指定のキーの値を取得
        //$value = $request->session()->get('account_key');

        //指定したデータをセッションから削除
        //$request->session()->forget('account_key');

        //セッションのすべてのデータを削除
        //$request->session()->flush();

        //  セッションに指定したキーが存在するかどうか判定
        //if($request->session()->exists('account_key')){
        //Debugbar::info($value);
        //}

        //デバッグ方法
        //dd($value);
        //Debugbar::info($value);
        //Debugbar::error('エラー');
    }

    // アカウント検索
    public function search(Request $request)
    {
        if ($request->name) {
            $accounts = Account::where('name', '=', $request['name'])->get();
            return view('layouts.app', ['accounts' => $accounts]);
        } else {
            return redirect()->route('accounts.index');
        }
    }

    //アカウント登録画面用
    public function create(Request $request)
    {
        return view('accounts.AccountRegistration');
    }

    //アカウント登録処理
    public function store(Request $request)
    {
        //バリデーションチェック
        $validated = $request->validate([
            'name' => ['required', 'min:4', 'max:25'],
            'password' => ['required', 'confirmed']
        ]);
        //条件を指定して取得
        $account = Account::where('name', '=', $request['name'])->get();
        // パスワードが一致していたら次の画面に遷移する
        if ($account->count() == 0) {
            //レコードの追加
            Account::create(['name' => $request['name'], 'password' => Hash::make($request['password'])]);
        } else {
            return redirect()->route('accountscreate', ['error' => 'invalid']);
        }

        //acconuts.indexにリダイレクト
        return redirect()->route('accountscompletion', ['name' => $request['name']]);
    }

    //アカウント登録完了のページにviewを送る
    public function completion(Request $request)
    {
        return view('accounts.AccountRegistrationcompletion', ['name' => $request['name']]);
    }

    //アカウント削除確認画面
    public function account_destroy(Request $request)
    {
        $accounts = Account::where('id', '=', $request['id'])->get();
        return view('accounts.accountdestroy', ['name' => $accounts[0]['name'], 'id' => $accounts[0]['id']]);
    }

    //アカウントを削除処理
    public function destroy(Request $request)
    {
        $account = Account::findOrFail($request['id']);
        $account->delete();

        return redirect()->route('accountsdestroy_complete', ['name' => $account['name']]);
    }

    //アカウント削除完了の関数
    public function destroy_complete(Request $request)
    {
        return view('accounts/destroycomplete', ['name' => $request['name']]);
    }

    //パスワード更新画面
    public function password_update(Request $request)
    {
        $accounts = Account::where('id', '=', $request['id'])->get();
        return view('accounts.update',
            ['name' => $accounts[0]['name'], 'id' => $accounts[0]['id'], 'password' => $accounts[0]['password']]);
    }

    //パスワード更新処理
    public function update(Request $request)
    {
        //パスワード一致のバリデーション
        $request->validate([
            'password' => ['required', 'confirmed']
        ]);
        $account = Account::findOrFail($request['id']);
        $account->password = Hash::make($request['password']);
        $account->save();
        return redirect()->route('accountsupdate_complete', ['name' => $account['name']]);
    }

    //パスワード更新完了の関数
    public function update_complete(Request $request)
    {
        return view('accounts/updatecomplete', ['name' => $request['name']]);
    }
}
