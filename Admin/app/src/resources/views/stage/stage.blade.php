@extends('layouts.app')
@section('title','ステージマスタ一覧')
@section('h1','ステージマスタ一覧')
@section('body')
    @if(!empty($stage))
        <table class="table table-bordered">
            <tr>
                <th>id</th>
                <th>残り手数</th>
                <th>制限時間</th>
            </tr>
            @foreach($stage as $stages)
                <tr>
                    <td>{{$stages['id']}}</td>
                    <td>{{$stages['hand_num']}}</td>
                    <td>{{$stages['time_limit']}}</td>
                </tr>
            @endforeach
        </table>
    @endif
@endsection



